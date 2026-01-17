using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Common.Interfaces;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Data;
using EMPLOYEE.Models;
using EmployeeDomain.Entities;
using EmployeeDomain.Exceptions;
using EmployeeDomain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMPLOYEE.Application.Features.Evaluations.Query
{
    public class EmployeeEvaluationService : IEmployeeEvaluationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IEvaluationRatingService _ratingService;
        private readonly IPermissionService _permissionService;

        public EmployeeEvaluationService(ApplicationDbContext db, IEvaluationRatingService ratingService, IPermissionService permissionService)
        {
            _db = db;
            _ratingService = ratingService;
            _permissionService = permissionService;
        }
        private async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _db.Employees.AnyAsync(e => e.Id == employeeId);
        }

        // ======================== Get By Id ========================
        public async Task<EvaluationDto?> GetByIdAsync(int id, string? callerId, bool isAdmin)
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Evaluations.View");
            if (!canManage)
            {
                throw new ForbiddenException("You do not have permission to view evaluations.");
            }
                var eval = await (
                from e in _db.EmployeeEvaluations
                join emp in _db.Employees on e.EmployeeId equals emp.Id
                join usr in _db.Users on emp.UserId equals usr.Id
                join dep in _db.Departments on emp.DepartmentId equals dep.Id
                join mgr in _db.Users on e.ManagerId equals mgr.Id
                where e.Id == id
                select new
                {
                    Evaluation = e,
                    EmployeeName = usr.FirstName + " " + usr.LastName, 
                    EmployeeManagerUserId = dep.ManagerUserId,
                    ManagerName = mgr.FirstName + " " + mgr.LastName
                }
            ).FirstOrDefaultAsync();

            if (eval == null)
                return null;

            if (!isAdmin && eval.EmployeeManagerUserId != callerId)
                return null;

            return new EvaluationDto
            {
                EmployeeName = eval.EmployeeName,
                ManagerName = eval.ManagerName,
                Attendance = eval.Evaluation.Attendance,
                Quality = eval.Evaluation.Quality,
                Productivity = eval.Evaluation.Productivity,
                Teamwork = eval.Evaluation.Teamwork,
                Comments = eval.Evaluation.Comments,
                FinalScore = eval.Evaluation.FinalScore,
                CreatedAt = eval.Evaluation.CreatedAt
            };
        }

        // ======================== My Evaluations ========================
        public async Task<EmployeeEvaluationsResponseDto?> GetMyEvaluationsAsync(string userId)
        {
        
            var data = await (from ev in _db.EmployeeEvaluations
                              join emp in _db.Employees on ev.EmployeeId equals emp.Id
                              join usr in _db.Users on emp.UserId equals usr.Id
                              join mgr in _db.Users on ev.ManagerId equals mgr.Id
                              where emp.UserId == userId
                              orderby ev.CreatedAt descending
                              select new
                              {
                                  ev.EmployeeId,
                                  EmployeeName = usr.FirstName + " " + usr.LastName,
                                  EmployeeEmail = usr.Email,
                                  ManagerName = mgr.FirstName + " " + mgr.LastName,
                                  Evaluation = new EvaluationItemDto
                                  {
                                      Attendance = ev.Attendance,
                                      Quality = ev.Quality,
                                      Productivity = ev.Productivity,
                                      Teamwork = ev.Teamwork,
                                      Comments = ev.Comments,
                                      Period = ev.Period,
                                      FinalScore = ev.FinalScore,
                                      CreatedAt = ev.CreatedAt
                                  }
                              }).ToListAsync();

            if (!data.Any()) return null;

            return new EmployeeEvaluationsResponseDto
            {
                EmployeeId = data.First().EmployeeId,
                EmployeeName = data.First().EmployeeName,
                EmployeeEmail = data.First().EmployeeEmail,
                ManagerName = data.First().ManagerName,
                Evaluations = data.Select(x => x.Evaluation).ToList()
            };
        }

        // ======================== Team Evaluations ========================
        public async Task<List<EmployeeEvaluationDashboardDto>> GetTeamEvaluationsAsync(
           string callerId,
           bool isAdmin,
           int? year = null,
           int? month = null
       )
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Evaluations.View");
            if (!canManage)
            {
                throw new ForbiddenException("You do not have permission to view evaluations.");
            }
            var query =
                from ev in _db.EmployeeEvaluations
                join emp in _db.Employees on ev.EmployeeId equals emp.Id
                join dep in _db.Departments on emp.DepartmentId equals dep.Id
                join usr in _db.Users on emp.UserId equals usr.Id
                where isAdmin || dep.ManagerUserId == callerId
                select new
                {
                    ev.EmployeeId,
                    EmployeeName = usr.FirstName + " " + usr.LastName,
                    Metrics = new EvaluationMetricsDto
                    {
                        Attendance = ev.Attendance,
                        Quality = ev.Quality,
                        Productivity = ev.Productivity,
                        Teamwork = ev.Teamwork,
                        FinalScore = ev.FinalScore,
                        Period = ev.Period
                    }
                };

            if (year.HasValue)
                query = query.Where(x => x.Metrics.Period.Year == year.Value);

            if (month.HasValue)
                query = query.Where(x => x.Metrics.Period.Month == month.Value);

            var data = await query.ToListAsync();
            if (!data.Any()) return new();

            return data
                .GroupBy(x => x.EmployeeId)
                .Select(g =>
                {
                    var employeeName = g.First().EmployeeName;

                    if (month.HasValue)
                    {
                        var avg = BuildAverage(g.Select(x => x.Metrics),
                                               $"{year}-{month:00}");

                        return new EmployeeEvaluationDashboardDto
                        {
                            EmployeeId = g.Key,
                            EmployeeName = employeeName,
                            Monthly = new List<EmployeeEvaluationAverageDto> { avg }
                        };
                    }

                    var yearlyAvg = BuildAverage(g.Select(x => x.Metrics),
                                                 year?.ToString() ?? "All");

                    return new EmployeeEvaluationDashboardDto
                    {
                        EmployeeId = g.Key,
                        EmployeeName = employeeName,
                        Yearly = yearlyAvg
                    };
                })
                .ToList();
        }



        // ======================== Yearly Dashboard ========================
        public async Task<EmployeeYearlyDashboardDto?> GetEmployeeYearlyDashboardAsync(
            int employeeId,
            string? callerUid,
            string? callerRole,
            int? year = null
        )
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerUid, "Permissions.Evaluations.View");
            if (!canManage)
            {
                throw new ForbiddenException("You do not have permission to view evaluations.");
            }
            bool isAdmin = callerRole == "Admin";

            bool isManagerForEmployee = await _db.EmployeeEvaluations
                .AnyAsync(e => e.EmployeeId == employeeId && e.ManagerId == callerUid);

            if (!isAdmin && !isManagerForEmployee)
                throw new BusinessException("Not in your team");

            int targetYear = year ?? DateTime.UtcNow.Year;

            var evaluations = await _db.EmployeeEvaluations
                .Where(e =>
                    e.EmployeeId == employeeId &&
                    e.Period.Year == targetYear
                )
                .ToListAsync();

            if (!evaluations.Any())
                return null;

            var employee = await (
                  from e in _db.Employees
                  join usr in _db.Users on e.UserId equals usr.Id
                  where e.Id == employeeId
                  select new
                  {
                      Name = usr.FirstName + " " + usr.LastName
                  }
              ).FirstOrDefaultAsync();


            if (employee == null)
                return null;

            // ===================== Yearly =====================
            var yearly = new EmployeeEvaluationAverageDto
            {
                PeriodLabel = targetYear.ToString(),
                Attendance = Math.Round((decimal)evaluations.Average(x => x.Attendance), 2),
                Quality = Math.Round((decimal)evaluations.Average(x => x.Quality), 2),
                Productivity = Math.Round((decimal)evaluations.Average(x => x.Productivity), 2),
                Teamwork = Math.Round((decimal)evaluations.Average(x => x.Teamwork), 2),
                FinalScore = Math.Round(evaluations.Average(x => x.FinalScore), 2),
                Rating = _ratingService.GetRating(
                    Math.Round(evaluations.Average(x => x.FinalScore), 2))

            };

            // ===================== Quarterly =====================
            var quarterly = new List<EmployeeEvaluationAverageDto>();

            for (int q = 1; q <= 4; q++)
            {
                int startMonth = (q - 1) * 3 + 1;
                int endMonth = startMonth + 2;

                var quarterData = evaluations
                    .Where(e => e.Period.Month >= startMonth && e.Period.Month <= endMonth)
                    .ToList();

                if (!quarterData.Any())
                {
                    quarterly.Add(new EmployeeEvaluationAverageDto
                    {
                        PeriodLabel = $"Q{q} {targetYear}",
                        Rating = "No Data"
                    });
                    continue;
                }

                quarterly.Add(new EmployeeEvaluationAverageDto
                {
                    PeriodLabel = $"Q{q} {targetYear}",
                    Attendance = Math.Round((decimal)quarterData.Average(x => x.Attendance), 2),
                    Quality = Math.Round((decimal)quarterData.Average(x => x.Quality), 2),
                    Productivity = Math.Round((decimal)quarterData.Average(x => x.Productivity), 2),
                    Teamwork = Math.Round((decimal)quarterData.Average(x => x.Teamwork), 2),
                    FinalScore = Math.Round(quarterData.Average(x => x.FinalScore), 2),
                    Rating = _ratingService.GetRating(
                     Math.Round(quarterData.Average(x => x.FinalScore), 2))
                });
            }

            return new EmployeeYearlyDashboardDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.Name,
                Yearly = yearly,
                Quarterly = quarterly
            };
        }

        // ======================== Monthly Dashboard ========================
        public async Task<EmployeeEvaluationDashboardDto?> GetEmployeeMonthlyDashboardAsync(
        int employeeId,
        string? callerUid,
        string? callerRole,
        int? year = null
    )
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerUid, "Permissions.Evaluations.View");
            if (!canManage)
            {
                throw new ForbiddenException("You do not have permission to view evaluations.");
            }
            bool isAdmin = callerRole == "Admin";

            bool isManagerForEmployee = await _db.EmployeeEvaluations
                .AnyAsync(e => e.EmployeeId == employeeId && e.ManagerId == callerUid);

            if (!isAdmin && !isManagerForEmployee)
                throw new BusinessException("Not in your team");

            int targetYear = year ?? DateTime.UtcNow.Year;

            var evaluations = await _db.EmployeeEvaluations
                .Where(e =>
                    e.EmployeeId == employeeId &&
                    e.Period.Year == targetYear
                )
                .ToListAsync();

            if (!evaluations.Any())
                return null;

            var employee = await (
                  from e in _db.Employees
                  join usr in _db.Users on e.UserId equals usr.Id
                  where e.Id == employeeId
                  select new
                  {
                      Name = usr.FirstName + " " + usr.LastName
                  }
              ).FirstOrDefaultAsync();

            if (employee == null)
                return null;

            var monthly = new List<EmployeeEvaluationAverageDto>();

            for (int month = 1; month <= 12; month++)
            {
                var monthData = evaluations
                    .Where(e => e.Period.Month == month)
                    .ToList();

                if (!monthData.Any())
                {
                    monthly.Add(new EmployeeEvaluationAverageDto
                    {
                        PeriodLabel = $"{targetYear}-{month:D2}",
                        Rating = "No Data"
                    });
                    continue;
                }

                monthly.Add(new EmployeeEvaluationAverageDto
                {
                    PeriodLabel = $"{targetYear}-{month:D2}",
                    Attendance = Math.Round((decimal)monthData.Average(x => x.Attendance), 2),
                    Quality = Math.Round((decimal)monthData.Average(x => x.Quality), 2),
                    Productivity = Math.Round((decimal)monthData.Average(x => x.Productivity), 2),
                    Teamwork = Math.Round((decimal)monthData.Average(x => x.Teamwork), 2),
                    FinalScore = Math.Round(monthData.Average(x => x.FinalScore), 2),
                    Rating = _ratingService.GetRating(
                     Math.Round(monthData.Average(x => x.FinalScore), 2))

                });
            }

            return new EmployeeEvaluationDashboardDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.Name,
                Monthly = monthly
            };
        }

        // ======================== Helper Method ========================
        private async Task<EmployeeEvaluationAverageDto?> CalculateAverageAsync(int employeeId, DateTime start, DateTime end, bool canShowRating)
        {
            var avg = await _db.EmployeeEvaluations
                .Where(ev => ev.EmployeeId == employeeId && ev.Period >= start && ev.Period < end)
                .GroupBy(ev => ev.EmployeeId)
                .Select(g => new
                {
                    Attendance = Math.Round((decimal)g.Average(x => x.Attendance), 2),
                    Quality = Math.Round((decimal)g.Average(x => x.Quality), 2),
                    Productivity = Math.Round((decimal)g.Average(x => x.Productivity), 2),
                    Teamwork = Math.Round((decimal)g.Average(x => x.Teamwork), 2),
                    FinalScore = Math.Round((decimal)g.Average(x => x.FinalScore), 2)
                })
                .FirstOrDefaultAsync();

            if (avg == null) return null;

            return new EmployeeEvaluationAverageDto
            {
                Attendance = avg.Attendance,
                Quality = avg.Quality,
                Productivity = avg.Productivity,
                Teamwork = avg.Teamwork,
                FinalScore = avg.FinalScore,
                Rating = _ratingService.GetRating(avg.FinalScore) 
            };
        }
        private EmployeeEvaluationAverageDto BuildAverage(
      IEnumerable<IEvaluationMetrics> data,
      string periodLabel
  )
        {
            var finalScore = Math.Round(data.Average(x => x.FinalScore), 2);

            return new EmployeeEvaluationAverageDto
            {
                PeriodLabel = periodLabel,
                Attendance = Math.Round((decimal)data.Average(x => x.Attendance), 2),
                Quality = Math.Round((decimal)data.Average(x => x.Quality), 2),
                Productivity = Math.Round((decimal)data.Average(x => x.Productivity), 2),
                Teamwork = Math.Round((decimal)data.Average(x => x.Teamwork), 2),
                FinalScore = finalScore,
                Rating = _ratingService.GetRating(finalScore)
            };
        }



    }
}
