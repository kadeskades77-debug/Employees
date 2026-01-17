using EmployeeDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Entities
{
    public class EmployeeEvaluation
    {
        public int Id { get; private set; }

        public int EmployeeId { get; private set; }

        public string ManagerId { get; private set; }  
        public DateTime Period { get; private set; }

        public int Attendance { get; private set; }
        public int Quality { get; private set; }
        public int Productivity { get; private set; }
        public int Teamwork { get; private set; }

        public decimal FinalScore { get; private set; }
        public string Comments { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private EmployeeEvaluation() { } // EF

        public static EmployeeEvaluation Create(
            int employeeId,
            string managerId,
            DateTime period,
            int attendance,
            int quality,
            int productivity,
            int teamwork,
            string? comments
        )
        {
            ValidateCriteria(attendance, quality, productivity, teamwork);

            var finalScore = CalculateFinal(
                attendance, quality, productivity, teamwork);

            return new EmployeeEvaluation
            {
                EmployeeId = employeeId,
                ManagerId = managerId,
                Period = period,

                Attendance = attendance,
                Quality = quality,
                Productivity = productivity,
                Teamwork = teamwork,

                FinalScore = finalScore,
                Comments = comments ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

        }


        public void Update(int attendance, int quality, int productivity, int teamwork, string? comments)
        {
            ValidateCriteria(attendance, quality, productivity, teamwork);

            Attendance = attendance;
            Quality = quality;
            Productivity = productivity;
            Teamwork = teamwork;
            FinalScore = CalculateFinal(attendance, quality, productivity, teamwork);
            Comments = comments ?? string.Empty;
            

        }
        private static void ValidateCriteria(
            int attendance, int quality, int productivity, int teamwork)
        {
            var values = new[] { attendance, quality, productivity, teamwork };

            if (values.Any(v => v < 1 || v > 5))
                throw new DomainException("Criteria must be between 1 and 5.");
        }

        private static decimal CalculateFinal(
            int attendance, int quality, int productivity, int teamwork)
        {
            return Math.Round(
                (attendance + quality + productivity + teamwork) / 4m, 2);
        }
        public void EnsureCanBeEdited(DateTime currentPeriod, bool isAdmin)
        {
            if (!isAdmin && Period < currentPeriod)
                throw new DomainException("Cannot edit evaluation after period ends");
        }
        public void EnsureCanBeUpdated(DateTime currentPeriod, bool isAdmin)
        {
            if (!isAdmin && Period < currentPeriod)
                throw new DomainException("Cannot update evaluation after period ends");
        }


        public void EnsureCanBeDeletedBy(string callerId, bool isAdmin)
        {
            if (isAdmin)
                return;

            if (ManagerId != callerId)
                throw new DomainException("You cannot delete this evaluation");
        }

    }


}
