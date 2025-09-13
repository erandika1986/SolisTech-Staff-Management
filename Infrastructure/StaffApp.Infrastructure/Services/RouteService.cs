using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Transport;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services;

public class RouteService(
    IStaffAppDbContext context,
    ILogger<RouteService> logger) : IRouteService
{
    public async Task<GeneralResponseDTO> SaveRouteAsync(RouteDTO model)
    {
        try
        {
            var route = await context.Routes
                        .FirstAsync(x => x.Id == model.Id);

            if (route is null)
            {
                route = new Domain.Entity.Transport.Route
                {
                    Name = model.Name,
                    Code = model.Code,
                    PickupPoint = string.Empty
                };

                context.Routes.Add(route);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Route created successfully."
                };
            }
            else
            {
                route.Name = model.Name;
                route.Code = model.Code;
                context.Routes.Update(route);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Route updated successfully."
                };
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating/updating route with Id: {RouteId}", model.Id);
            return new GeneralResponseDTO
            {
                Flag = false,
                Message = "An error occurred while processing the request."
            };
        }
    }

    public async Task<GeneralResponseDTO> DeleteRouteAsync(int id)
    {
        try
        {
            var route = await context.Routes
                .FirstOrDefaultAsync(x => x.Id == id);

            if (route is null)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = $"Route with Id {id} not found."
                };
            }

            context.Routes.Remove(route);
            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO
            {
                Flag = true,
                Message = "Route deleted successfully."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting route with Id: {RouteId}", id);
            return new GeneralResponseDTO
            {
                Flag = false,
                Message = "An error occurred while deleting the route."
            };
        }
    }

    public async Task<List<RouteDTO>> GetAllRoutesAsync(string name)
    {
        try
        {
            var query = context.Routes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            var routes = await query
                .Select(x => new RouteDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code
                })
                .ToListAsync();

            return routes;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching routes. Filter: {Name}", name);
            return new List<RouteDTO>();
        }
    }
}
