using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Vehicle;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity.Master;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class VehicleService(
        IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        ILogger<IVehicleService> logger) : IVehicleService
    {
        public async Task<GeneralResponseDTO> DeleteVehicle(int vehicleId)
        {
            var vehicle = context.Vehicles.Find(vehicleId);

            vehicle.IsActive = true;
            vehicle.UpdateDate = DateTime.UtcNow;
            vehicle.UpdatedByUserId = currentUserService.UserId;

            context.Vehicles.Update(vehicle);
            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO(true, "Vehicle deleted successfully.");
        }

        public async Task<List<VehicleDTO>> GetAllVehicles()
        {
            var activeVehicles = await context.Vehicles
                .Where(v => v.IsActive)
                .Select(v => new VehicleDTO
                {
                    Id = v.Id,
                    Registration = v.Registration,
                    Capacity = v.Capacity,
                    VehicleType = new DropDownDTO() { Id = (int)v.VehicleType, Name = EnumHelper.GetEnumDescription(v.VehicleType) },
                    VehicleOwner = new DropDownDTO() { Id = (int)v.VehicleOwner, Name = EnumHelper.GetEnumDescription(v.VehicleOwner) },
                    ManufactureName = v.ManufactureName,
                    MonthlyRent = v.MonthlyRent,
                    CreatedOn = v.CreatedDate.ToString("dd/MM/yyyy"),
                    AssignedPurposes = v.VehicleAssignedPurposes
                        .Select(vp => new DropDownDTO
                        {
                            Id = vp.VehiclePurposeId,
                            Name = vp.VehiclePurpose.Name,
                        }).ToList()
                }).ToListAsync();

            return activeVehicles;
        }

        public async Task<VehicleMasterDataDTO> GetVehicleMasterData()
        {
            var masterData = new VehicleMasterDataDTO
            {
                VehicleTypes = Enum.GetValues(typeof(Domain.Enum.VehicleType))
                    .Cast<Domain.Enum.VehicleType>()
                    .Select(vt => new DropDownDTO
                    {
                        Id = (int)vt,
                        Name = vt.ToString()
                    }).ToList(),
                VehicleOwnerTypes = Enum.GetValues(typeof(Domain.Enum.VehicleOwner))
                    .Cast<Domain.Enum.VehicleOwner>()
                    .Select(vo => new DropDownDTO
                    {
                        Id = (int)vo,
                        Name = vo.ToString()
                    }).ToList(),
                VehiclePurposes = await context.VehiclePurposes
                    .Select(vp => new DropDownDTO
                    {
                        Id = vp.Id,
                        Name = vp.Name
                    }).ToListAsync()
            };

            return masterData;
        }

        public async Task<GeneralResponseDTO> SaveVehicle(VehicleDTO vehicleDTO)
        {
            try
            {
                var vehicle = await context.Vehicles.FindAsync(vehicleDTO.Id);
                if (vehicle == null)
                {
                    vehicle = new Vehicle()
                    {
                        Capacity = vehicleDTO.Capacity,
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        ManufactureName = vehicleDTO.ManufactureName,
                        MonthlyRent = vehicleDTO.MonthlyRent,
                        Registration = vehicleDTO.Registration,
                        UpdateDate = DateTime.UtcNow,
                        UpdatedByUserId = currentUserService.UserId,
                        VehicleOwner = (VehicleOwner)vehicleDTO.VehicleOwner.Id,
                        VehicleType = (VehicleType)vehicleDTO.VehicleType.Id,
                    };

                    vehicle.VehicleAssignedPurposes = vehicleDTO.AssignedPurposes.Select(ap => new VehicleAssignedPurpose
                    {
                        VehiclePurposeId = ap.Id
                    }).ToList();

                    context.Vehicles.Add(vehicle);
                }
                else
                {
                    vehicle.Capacity = vehicleDTO.Capacity;
                    vehicle.UpdateDate = DateTime.UtcNow;
                    vehicle.UpdatedByUserId = currentUserService.UserId;
                    vehicle.ManufactureName = vehicleDTO.ManufactureName;
                    vehicle.MonthlyRent = vehicleDTO.MonthlyRent;
                    vehicle.Registration = vehicleDTO.Registration;
                    vehicle.VehicleOwner = (VehicleOwner)vehicleDTO.VehicleOwner.Id;
                    vehicle.VehicleType = (VehicleType)vehicleDTO.VehicleType.Id;

                    var existingPurposes = vehicle.VehicleAssignedPurposes.ToList();
                    var updatedPurpose = vehicleDTO.AssignedPurposes;

                    var purposesToRemove = existingPurposes
                        .Where(ep => !updatedPurpose.Any(up => up.Id == ep.VehiclePurposeId))
                        .ToList();

                    foreach (var purpose in purposesToRemove)
                    {
                        vehicle.VehicleAssignedPurposes.Remove(purpose);
                    }

                    var purposesToAdd = updatedPurpose.Where(up => !existingPurposes.Any(ep => ep.VehiclePurposeId == up.Id))
                        .Select(up => new VehicleAssignedPurpose
                        {
                            VehiclePurposeId = up.Id
                        }).ToList();

                    foreach (var purpose in purposesToAdd)
                    {
                        vehicle.VehicleAssignedPurposes.Add(purpose);
                    }

                    context.Vehicles.Update(vehicle);
                }

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO(true, "Vehicle saved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving vehicle");
                return new GeneralResponseDTO(false, "An error occurred while saving the vehicle.");
            }


        }
    }
}
