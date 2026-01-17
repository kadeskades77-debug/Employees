using EMPLOYEE.Application.Features.Employees.EmployeeDtos;
using EMPLOYEE.Data;
using EMPLOYEE.Models;
using EMPLOYEE.Service;
using Microsoft.EntityFrameworkCore;

namespace EMPLOYEE.Repository
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Department?> GetDepartmentForManagerAsync(string managerUserId)
        {
            return await _context.Departments
                .Where(d => d.ManagerUserId == managerUserId)
                .FirstOrDefaultAsync();
        }


        public async Task<List<EmployeeWithManagerDto>> GetAllEmployeesAsync()
        {
            var data =
                from e in _context.Employees
                join u in _context.Users on e.UserId equals u.Id
                join d in _context.Departments on e.DepartmentId equals d.Id
                join m in _context.Users on d.ManagerUserId equals m.Id
                select new
                {
                    EmployeeNumber = e.Id,
                    EmployeeName = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                    EmployeeEmail = u.Email,
                    DepartmentName = d.Name,
                    ManagerId = m.Id,
                    ManagerName = (m.FirstName ?? "") + " " + (m.LastName ?? "")
                };

            return await data
                .GroupBy(x => new { x.ManagerId, x.ManagerName })
                .Select(g => new EmployeeWithManagerDto
                {
                    ManagerName = g.Key.ManagerName,
                    Employees = g.Select(emp => new EmployeeDto
                    {
                        EmployeeNumber = emp.EmployeeNumber,
                        EmployeeName = emp.EmployeeName,
                        EmployeeEmail = emp.EmployeeEmail,
                        DepartmentName = emp.DepartmentName
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<List<EmployeeWithManagerDto>> GetByManagerAsync(string managerId)
        {
            var data =
                from e in _context.Employees
                join u in _context.Users on e.UserId equals u.Id
                join d in _context.Departments on e.DepartmentId equals d.Id
                join m in _context.Users on d.ManagerUserId equals m.Id
                where d.ManagerUserId == managerId
                select new
                {
                    EmployeeNumber = e.Id,
                    EmployeeName = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                    EmployeeEmail = u.Email,
                    DepartmentName = d.Name,
                    ManagerId = m.Id,
                    ManagerName = (m.FirstName ?? "") + " " + (m.LastName ?? "")
                };

            return await data
                .GroupBy(x => new { x.ManagerId, x.ManagerName })
                .Select(g => new EmployeeWithManagerDto
                {
                    ManagerName = g.Key.ManagerName,
                    Employees = g.Select(emp => new EmployeeDto
                    {
                        EmployeeNumber = emp.EmployeeNumber,
                        EmployeeName = emp.EmployeeName,
                        EmployeeEmail = emp.EmployeeEmail,
                        DepartmentName = emp.DepartmentName
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<EmployeeDto?> GetByEmailAsync(string email)
        {
            return await (
                from e in _context.Employees
                join u in _context.Users on e.UserId equals u.Id
                join d in _context.Departments on e.DepartmentId equals d.Id into dep
                from d in dep.DefaultIfEmpty()
                where u.Email == email
                select new EmployeeDto
                {
                    EmployeeNumber = e.Id,
                    EmployeeName = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                    EmployeeEmail = u.Email,
                    DepartmentName = d != null ? d.Name : null,
                    Position = e.Position,
                    Salary = e.Salary
                }
            ).FirstOrDefaultAsync();
        }
        public async Task<bool> CanManageEmployeeAsync(int employeeId, string callerId, string callerRole)
        {
           
            if (callerRole == "Admin")
                return true;
            
            var employee = await GetByIdAsync(employeeId);
            if (employee == null) return false;

           
            var department = await GetDepartmentForManagerAsync(callerId);
            return department != null;
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == departmentId);
        }

        public async Task<bool> HasCurrentMonthEvaluationAsync(int employeeId)
{
    var currentPeriod = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    return await _context.EmployeeEvaluations
        .AnyAsync(e => e.EmployeeId == employeeId
                       && e.Period.Year == currentPeriod.Year
                       && e.Period.Month == currentPeriod.Month);
}




    }

}
