namespace EMPLOYEE.Application.Features.Employees.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string? Position { get; set; } 
        public decimal? Salary { get; set; } 
    }
}
