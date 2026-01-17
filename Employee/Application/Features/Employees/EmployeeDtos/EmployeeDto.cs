namespace EMPLOYEE.Application.Features.Employees.EmployeeDtos
{
    public class EmployeeDto
    {
        public int EmployeeNumber { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; } = 0;

    }
}
