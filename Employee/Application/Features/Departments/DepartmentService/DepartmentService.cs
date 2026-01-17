using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Features.Departments.DepartmentsDtos;
using EMPLOYEE.Data;
using EmployeeDomain.Entities;
using EMPLOYEE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMPLOYEE.Application.Features.Departments.DepartmentService
{
           public class DepartmentService : IDepartmentService
        {
            private readonly ApplicationDbContext _db;
             private readonly UserManager<ApplicationUser> _userManager;
            private readonly IPermissionService _permissionService;

        public DepartmentService(ApplicationDbContext db, IPermissionService permissionService, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _permissionService = permissionService;
            _userManager = userManager;
        }

        // ===================== Create =====================

        public async Task<string> CreateAsync(
                CreateDepartmentDto dto,
                string callerId)
            {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Departments.Manage");
            if (!canManage)
                return "You do not have permission to manage departments.";


            var exists = await IsManagerAssignedToAnotherDepartmentAsync(dto.ManagerUserId);

            if (exists)
                return "This manager is already assigned to another department";


            var department = new Department
                {
                    Name = dto.Name,
                    ManagerUserId = dto.ManagerUserId
                };

                _db.Departments.Add(department);
                await _db.SaveChangesAsync();

                return "Department created successfully";
            }

            // ===================== Update =====================

            public async Task<string> UpdateAsync(
                int departmentId,
                UpdateDepartmentDto dto,
                string callerId)
            {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Departments.Manage");
            if (!canManage)
                return "You do not have permission to manage departments.";

                var department = await _db.Departments
                    .FirstOrDefaultAsync(d => d.Id == departmentId);

                if (department == null)
                    return "Department not found";
        
            department.Name = dto.Name;

                await _db.SaveChangesAsync();
                return "Department updated successfully";
            }

        // ===================== Change Manager =====================

        public async Task<string> ChangeManagerAsync(ChangeDepartmentManagerDto dto, string callerId)
        {
      
            if (string.IsNullOrEmpty(callerId))
                return "User ID not found in token.";

          
            if (dto == null || string.IsNullOrEmpty(dto.NewManagerUserId) || dto.DepartmentId <= 0)
                return "Invalid request data.";

          
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Departments.Manage");
            if (!canManage)
                return "You do not have permission to manage departments.";

           
            var department = await _db.Departments.FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);
            if (department == null)
                return "Department not found";

         
            var exists = await IsManagerAssignedToAnotherDepartmentAsync(dto.NewManagerUserId);
            if (exists)
                return "This manager is already assigned to another department";

          
            if (department.ManagerUserId == dto.NewManagerUserId)
                return "This user is already the manager of this department";

          
            var manager = await _userManager.FindByIdAsync(dto.NewManagerUserId);
            if (manager == null)
                return "User not found";

            if (!await _userManager.IsInRoleAsync(manager, "Manager"))
                return "User must have Manager role";

          
            department.ManagerUserId = dto.NewManagerUserId;
            await _db.SaveChangesAsync();

            return "Department manager updated successfully";
        }


        // ===================== Get All =====================

        public async Task<List<DepartmentDto>> GetAllAsync()
            {
                return await (
                    from d in _db.Departments
                    join u in _db.Users on d.ManagerUserId equals u.Id
                    select new DepartmentDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        ManagerUserId = u.Id,
                        ManagerName = u.FirstName + " " + u.LastName
                    }
                ).ToListAsync();
            }

        // ===================== Get My Department =====================

            public async Task<DepartmentDto?> GetMyDepartmentAsync(string managerUserId)
        {
            return await (
                from d in _db.Departments
                join u in _db.Users on d.ManagerUserId equals u.Id
                where u.Id == managerUserId
                select new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerUserId = u.Id,
                    ManagerName = u.FirstName + " " + u.LastName
                }
            ).FirstOrDefaultAsync();
        }

        // ===================== Help Method =====================
            public async Task<bool> IsManagerAssignedToAnotherDepartmentAsync(
             string managerUserId,
               int? excludeDepartmentId = null)
        {
            return await _db.Departments.AnyAsync(d =>
                d.ManagerUserId == managerUserId &&
                (excludeDepartmentId == null || d.Id != excludeDepartmentId)
            );
        }

    }
}


