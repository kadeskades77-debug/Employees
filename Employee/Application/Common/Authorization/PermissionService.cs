using EMPLOYEE.Application.Dtos;
using EMPLOYEE.Data;
using EMPLOYEE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMPLOYEE.Application.Common.Authorization
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionService(

            UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<string> AddPermissionAsync(AddPermissionDto dto)
        {
            var exists = await _context.Permissions
                .AnyAsync(p => p.Name == dto.Name);

            if (exists)
                return "Permission already exists";

            _context.Permissions.Add(new Permissions
            {
                Name = dto.Name
            });

            await _context.SaveChangesAsync();
            return "Permission added successfully";
        }

        // ================= Assign Permission To Role =================

        public async Task<string> AssignPermissionToRoleAsync(AssignPermissionDto dto)
        {
            var role = await _roleManager.FindByNameAsync(dto.RoleName);
            if (role == null)
                return "Role not found";

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == dto.PermissionName);

            if (permission == null)
                return "Permission not found";

            bool exists = await _context.RolePermissions.AnyAsync(rp =>
                rp.RoleId == role.Id &&
                rp.PermissionId == permission.Id);

            if (exists)
                return "Permission already assigned to this role";

            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });

            await _context.SaveChangesAsync();
            return "Permission assigned to role successfully";
        }
        public async Task<bool> HasPermissionAsync(string userId, string permissionName)
        {
            var permissions = await GetPermissionsAsync(userId);
            return permissions.Contains(permissionName);
        }


        public async Task<List<string>> GetPermissionsAsync(string userId)
        {
           
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user); 

           
            var permissions = await (
                from rp in _context.RolePermissions
                join r in _context.Roles on rp.RoleId equals r.Id
                join p in _context.Permissions on rp.PermissionId equals p.Id
                where roles.Contains(r.Name) 
                select p.Name
            )
            .Distinct()
            .ToListAsync();

            return permissions;
        }

    }

}
