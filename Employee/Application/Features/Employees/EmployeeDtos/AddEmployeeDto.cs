using EMPLOYEE.Models;

namespace EMPLOYEE.Application.Features.Employees.EmployeeDtos
{
    public class AddEmployeeDto
    {
        public string FirstName { get; set; }= string.Empty;
        public string LastName { get; set; }= string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; } = 0;

    }
}
