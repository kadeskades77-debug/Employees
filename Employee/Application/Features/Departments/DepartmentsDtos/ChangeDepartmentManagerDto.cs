namespace EMPLOYEE.Application.Features.Departments.DepartmentsDtos
{
    public class ChangeDepartmentManagerDto
    {
        public int DepartmentId { get; set; }
        public string NewManagerUserId { get; set; } = default!;
    }

}
