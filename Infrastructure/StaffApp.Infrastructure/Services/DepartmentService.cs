using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Department;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IStaffAppDbContext context;
        private readonly ICurrentUserService currentUserService;

        public DepartmentService(IStaffAppDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<GeneralResponseDTO> SaveDepartment(DepartmentDTO model)
        {
            var department = await context.Departments.FindAsync(model.Id);
            if (department != null)
            {
                department.Name = model.Name;

                if (model.DepartmentHead is not null)
                {
                    department.DepartmentHeadId = model.DepartmentHead.Id;
                }
                else
                {
                    department.DepartmentHeadId = null;
                }

                context.Departments.Update(department);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO { Flag = true, Message = "Department updated successfully" };
            }
            else
            {
                department = new Domain.Entity.Department
                {
                    Name = model.Name,
                };

                if (model.DepartmentHead is not null)
                {
                    department.DepartmentHeadId = model.DepartmentHead.Id;
                }

                context.Departments.Add(department);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO { Flag = true, Message = "Department added successfully" };
            }
        }

        public async Task<GeneralResponseDTO> DeleteDepartment(int id)
        {
            var department = await context.Departments.FindAsync(id);

            if (department == null)
                return new GeneralResponseDTO { Flag = false, Message = "Department not found" };

            try
            {
                department.IsActive = false;
                context.Departments.Update(department);
                await context.SaveChangesAsync(CancellationToken.None);
                return new GeneralResponseDTO { Flag = true, Message = "Department deleted successfully" };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO { Flag = false, Message = ex.Message };
            }
        }

        public async Task<List<DepartmentDTO>> GetAllDepartments(string searchText, bool isActive = true)
        {
            IQueryable<Department> query = context
                .Departments
                .OrderBy(x => x.Name);

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(x => x.Name.Contains(searchText));

            var departments = await query.Where(x => x.IsActive == isActive).ToListAsync();

            return departments.Select(d => new DepartmentDTO
            {
                Id = d.Id,
                Name = d.Name,
                DepartmentHeadId = d.DepartmentHeadId,
                DepartmentHeadName = d.DepartmentHead != null ? d.DepartmentHead.FullName : string.Empty,
                AssignedEmployeeCount = d.EmployeeDepartments.Where(x => x.IsActive).Count()
            }).OrderBy(x => x.Name)
            .ToList();
        }

        public async Task<DepartmentDTO> GetDepartmentById(int id)
        {
            var department = await context.Departments.FindAsync(id);

            return department != null ? new DepartmentDTO
            {
                Id = department.Id,
                Name = department.Name,
                DepartmentHeadId = department.DepartmentHeadId,
                DepartmentHead = department.DepartmentHead != null ? new UserDropDownDTO
                {
                    Id = department.DepartmentHead.Id,
                    Name = department.DepartmentHead.FullName
                } : null,
                DepartmentHeadName = department.DepartmentHead != null ? department.DepartmentHead.FullName : string.Empty
            } : null;
        }

        public async Task<List<DepartmentDTO>> GetDepartmentsByUserId(string userId, bool isActive)
        {
            var employeeDepartments = await context.EmployeeDepartments
                .Where(x => x.UserId == userId && x.IsActive == isActive)
                .ToListAsync();

            var assignedDepartment = employeeDepartments
                .Select(x => new DepartmentDTO
                {
                    Id = x.DepartmentId,
                    Name = x.Department.Name,
                    DepartmentHeadId = x.Department.DepartmentHeadId,
                    DepartmentHeadName = x.Department.DepartmentHead != null ? x.Department.DepartmentHead.FullName : string.Empty
                })
                .ToList();

            return assignedDepartment;
        }

        public async Task<GeneralResponseDTO> AddUsersToDepartments(List<int> departments, string userId)
        {
            foreach (var departmentId in departments)
            {
                var userDepartment = new EmployeeDepartment()
                {
                    DepartmentId = departmentId,
                    UserId = userId,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    IsActive = true

                };

                await context.EmployeeDepartments.AddAsync(userDepartment);
            }

            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO { Flag = true, Message = "User added to departments successfully" };
        }

        public async Task<GeneralResponseDTO> RemoveUsersFromDepartments(List<int> departments, string userId)
        {
            foreach (var departmentId in departments)
            {
                var userDepartment = context.EmployeeDepartments.FirstOrDefault(x => x.DepartmentId == departmentId && x.UserId == userId);
                if (userDepartment != null)
                {
                    userDepartment.IsActive = false;
                    userDepartment.UpdateDate = DateTime.Now;
                    userDepartment.UpdatedByUserId = currentUserService.UserId;
                }

                context.EmployeeDepartments.Update(userDepartment);
            }

            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO { Flag = true, Message = "User removed from departments successfully" };
        }


    }
}
