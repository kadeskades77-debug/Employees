namespace EMPLOYEE.Application.Features.Departments.DepartmentsDtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string ManagerUserId { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
    }

}
