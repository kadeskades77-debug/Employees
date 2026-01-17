namespace EMPLOYEE.Application.Features.Employees.EmployeeDtos
{
    public class EmployeeWithManagerDto
    {
        public string ManagerName { get; set; } = string.Empty;
        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
}
