namespace EMPLOYEE.Application.Features.Departments.DepartmentsDtos
{
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string ManagerUserId { get; set; } = string.Empty;
    }

}
