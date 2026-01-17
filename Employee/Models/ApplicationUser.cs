using EmployeeDomain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EMPLOYEE.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required,MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        ICollection<EmployeeEvaluation> Evaluations { get; set; }

    }
}
