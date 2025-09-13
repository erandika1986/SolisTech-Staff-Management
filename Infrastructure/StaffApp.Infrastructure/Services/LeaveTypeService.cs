using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Leave;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity.Leave;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveTypeService(IStaffAppDbContext context) : ILeaveTypeService
    {
        public async Task<GeneralResponseDTO> DeleteLeaveType(int id)
        {
            var leaveType = await context.LeaveTypes.FindAsync(id);
            leaveType.IsActive = false;

            context.LeaveTypes.Update(leaveType);
            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO() { Flag = true, Message = "Leave type deleted successfully." };
        }

        public async Task<List<LeaveTypeDTO>> GetAllLeaveType()
        {
            var leaveType = await context
                .LeaveTypes
                .Select(x => new LeaveTypeDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    DefaultDays = x.DefaultDays,
                    HasLeaveTypeLogic = x.HasLeaveTypeLogic,
                    AllowGenderTypeName = x.AllowGenderType == ApplicationConstants.Zero ? "All" : x.AllowGenderType == (int)Gender.Male ? "Male" : "Female",
                    LeaveTypeLogic = x.HasLeaveTypeLogic ? "YES" : "NO"
                })
                .ToListAsync();

            return leaveType;
        }

        public async Task<List<LeaveTypeConfigurationDTO>> GetLeaveTypeConfigurationId(int leaveTypeId)
        {
            var response = new List<LeaveTypeConfigurationDTO>();

            var employmentTypes = await context.EmployeeTypes.ToListAsync();

            foreach (var employmentType in employmentTypes)
            {
                var leaveTypeConfig = await context.LeaveTypesConfigs
                    .FirstOrDefaultAsync(x => x.EmployeeTypeId == employmentType.Id && x.LeaveTypeId == leaveTypeId);

                if (leaveTypeConfig == null)
                {
                    var leaveType = await context.LeaveTypes.FindAsync(leaveTypeId);

                    response.Add(new LeaveTypeConfigurationDTO()
                    {
                        AnnualLeaveAllowance = ApplicationConstants.Zero,
                        LeaveTypeId = leaveTypeId,
                        EmployeeTypeId = employmentType.Id,
                        EmployeeTypeName = employmentType.Name,
                        Id = ApplicationConstants.Zero,
                        LeaveTypeName = leaveType.Name,
                        MinimumServiceMonthsRequired = ApplicationConstants.Zero
                    });
                }
                else
                {
                    response.Add(new LeaveTypeConfigurationDTO()
                    {
                        AnnualLeaveAllowance = leaveTypeConfig.AnnualLeaveAllowance,
                        LeaveTypeId = leaveTypeId,
                        EmployeeTypeId = leaveTypeConfig.EmployeeTypeId,
                        EmployeeTypeName = leaveTypeConfig.EmployeeType.Name,
                        Id = leaveTypeConfig.Id,
                        LeaveTypeName = leaveTypeConfig.LeaveType.Name,
                        MinimumServiceMonthsRequired = leaveTypeConfig.MinimumServiceMonthsRequired.HasValue ? leaveTypeConfig.MinimumServiceMonthsRequired.Value : ApplicationConstants.Zero
                    });
                }
            }

            return response;
        }

        public async Task<GeneralResponseDTO> SaveLeaveTypeConfiguration(List<LeaveTypeConfigurationDTO> models)
        {
            try
            {
                foreach (var model in models)
                {
                    var leaveTypeConfig = await context.LeaveTypesConfigs.FindAsync(model.Id);

                    if (leaveTypeConfig is null)
                    {
                        leaveTypeConfig = new LeaveTypeConfig()
                        {
                            AnnualLeaveAllowance = model.AnnualLeaveAllowance,
                            LeaveTypeId = model.LeaveTypeId,
                            EmployeeTypeId = model.EmployeeTypeId,
                            MinimumServiceMonthsRequired = model.MinimumServiceMonthsRequired
                        };

                        context.LeaveTypesConfigs.Add(leaveTypeConfig);
                    }
                    else
                    {
                        leaveTypeConfig.AnnualLeaveAllowance = model.AnnualLeaveAllowance;
                        leaveTypeConfig.EmployeeTypeId = model.EmployeeTypeId;
                        leaveTypeConfig.MinimumServiceMonthsRequired = model.MinimumServiceMonthsRequired;

                        context.LeaveTypesConfigs.Update(leaveTypeConfig);
                    }
                }

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Leave type configurations saved successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<LeaveTypeDTO> GetLeaveTypeId(int id)
        {
            var leaveType = await context.LeaveTypes.FindAsync(id);

            return new LeaveTypeDTO()
            {
                Id = leaveType.Id,
                Name = leaveType.Name,
                DefaultDays = leaveType.DefaultDays,
                HasLeaveTypeLogic = leaveType.HasLeaveTypeLogic,
                AllowGenderTypeName = leaveType.AllowGenderType == ApplicationConstants.Zero ? "All" : leaveType.AllowGenderType == (int)Gender.Male ? "Male" : "Female",
                SelectedAllowGenderType = new DropDownDTO() { Id = leaveType.AllowGenderType },
                LeaveTypeLogic = leaveType.HasLeaveTypeLogic ? "YES" : "NO"
            };
        }

        public async Task<GeneralResponseDTO> SaveLeaveType(LeaveTypeDTO model)
        {
            var leaveType = await context.LeaveTypes.FindAsync(model.Id);

            if (leaveType == null)
            {
                leaveType = new LeaveType()
                {
                    Name = model.Name,
                    DefaultDays = model.DefaultDays,
                    HasLeaveTypeLogic = model.HasLeaveTypeLogic,
                    AllowGenderType = model.SelectedAllowGenderType.Id,
                    IsActive = true
                };

                context.LeaveTypes.Add(leaveType);

                await context.SaveChangesAsync(CancellationToken.None);
                return new GeneralResponseDTO() { Flag = true, Message = "A new leave type added successfully." };
            }
            else
            {
                leaveType.Name = model.Name;
                leaveType.DefaultDays = model.DefaultDays;
                leaveType.HasLeaveTypeLogic = model.HasLeaveTypeLogic;
                leaveType.AllowGenderType = model.SelectedAllowGenderType.Id;

                context.LeaveTypes.Update(leaveType);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Leave type updated successfully." };
            }
        }
    }
}
