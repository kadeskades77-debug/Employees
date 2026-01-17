using System.Security;

namespace EMPLOYEE.Application.Common.Authorization
{
    public class RolePermission
    {
        public string RoleId { get; set; } = null!;
        public int PermissionId { get; set; }

        public Permissions Permission { get; set; } = null!;
    }

}
