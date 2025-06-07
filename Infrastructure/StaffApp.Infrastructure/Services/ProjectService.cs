using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Project;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class ProjectService(IStaffAppDbContext context,
        IUserService userService,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        ILogger<IProjectService> logger) : IProjectService
    {
        public async Task<GeneralResponseDTO> AddProjectDocument(ProjectDocumentAttachmentDTO projectDocumentAttachment)
        {
            try
            {
                long maxFileSize = 10 * 1024 * 1024;

                var project = await context.Projects.FindAsync(projectDocumentAttachment.ProjectId);

                for (int i = 0; i < projectDocumentAttachment.Files.Count; i++)
                {
                    var extension = Path.GetExtension(projectDocumentAttachment.Files[i].Name);

                    var fileName = $"{Path.GetFileNameWithoutExtension(projectDocumentAttachment.Files[i].Name)}{extension}";
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";

                    await using var stream = projectDocumentAttachment.Files[i].OpenReadStream(maxFileSize);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var uploadedFileUrl = await azureBlobService
                        .UploadFileAsync(memoryStream, uniqueFileName, projectDocumentAttachment.Files[i].ContentType, ApplicationConstants.AzureBlobStorageName);


                    project.ProjectDocuments.Add(new ProjectDocument()
                    {
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        OriginalFileName = fileName,
                        SavedFileName = uniqueFileName,
                        SavedPath = uploadedFileUrl,
                        UpdateDate = DateTime.Now,
                        UpdatedByUserId = currentUserService.UserId,
                    });

                    context.Projects.Update(project);
                    await context.SaveChangesAsync(CancellationToken.None);
                }

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Project documents have been uploaded successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving project");
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while Uploading the project documents."
                };
            }
        }

        public async Task<GeneralResponseDTO> AddProjectMember(int projectId, string userId, string roleId)
        {
            try
            {
                var projectMember = new ProjectMember
                {
                    ProjectId = projectId,
                    MemberId = userId,
                    RoleId = roleId,
                    IsActive = true,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.UtcNow
                };

                context.ProjectMembers.Add(projectMember);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Project member has assigned to project successfully."
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving project");
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the project."
                };
            }
        }
        public async Task<List<UserDropDownDTO>> GetAvailableProjectMembers(int projectId)
        {
            var assignedMembers = await context.ProjectMembers
                .Where(x => x.ProjectId == projectId && x.IsActive)
                .Select(x => x.MemberId)
                .ToListAsync();

            var users = await context.ApplicationUsers
                .Where(x => x.IsActive && !assignedMembers.Contains(x.Id))
                .Select(x => new UserDropDownDTO
                {
                    Id = x.Id,
                    Name = x.FullName
                }).ToListAsync();

            return users;
        }

        public async Task<GeneralResponseDTO> DeleteProject(int projectId)
        {
            try
            {
                var project = await context.Projects.FindAsync(projectId);
                project.IsActive = false;
                project.UpdatedByUserId = currentUserService.UserId;
                project.UpdateDate = DateTime.UtcNow;

                context.Projects.Update(project);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Project has deleted successfully."
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving project");
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the project."
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteProjectDocument(int projectDocumentId)
        {
            try
            {
                var projectDocument = await context.ProjectDocuments.FindAsync(projectDocumentId);
                projectDocument.IsActive = false;
                projectDocument.UpdatedByUserId = currentUserService.UserId;
                projectDocument.UpdateDate = DateTime.UtcNow;

                context.ProjectDocuments.Update(projectDocument);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Project document has deleted successfully."
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving project");
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the project."
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteProjectMember(int projectMemberId)
        {
            try
            {
                var projectMember = await context.ProjectMembers.FindAsync(projectMemberId);
                projectMember.IsActive = false;
                projectMember.UpdatedByUserId = currentUserService.UserId;
                projectMember.UpdateDate = DateTime.UtcNow;

                context.ProjectMembers.Update(projectMember);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Project member has deleted successfully."
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving project");
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the project."
                };
            }
        }

        public async Task<PaginatedResultDTO<ProjectDTO>> GetAllProjectsAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true)
        {
            bool isReadOnly = false;
            var query = context.Projects.Where(x => x.IsActive == true);

            var assignedRoles = await userService
                .GetLoggedInUserAssignedRoles(currentUserService.UserId);


            if (!assignedRoles.Any(x => x == RoleConstants.Admin) &&
                !assignedRoles.Any(x => x == RoleConstants.Director) &&
                (assignedRoles.Any(x => x == RoleConstants.Manager) || assignedRoles.Any(x => x == RoleConstants.TeamLead)))
            {
                query = query
                    .Where(x => x.ManagerId == currentUserService.UserId);
            }

            if (!assignedRoles.Any(x => x == RoleConstants.Admin) &&
                !assignedRoles.Any(x => x == RoleConstants.Director) &&
                !assignedRoles.Any(x => x == RoleConstants.Manager) &&
                !assignedRoles.Any(x => x == RoleConstants.TeamLead) &&
                (
                    assignedRoles.Any(x => x == RoleConstants.AssociateQAEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.QAEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.SeniorQAEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.TraineeQAEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.SeniorSoftwareEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.SoftwareEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.TraineeSoftwareEngineer) ||
                    assignedRoles.Any(x => x == RoleConstants.AssociateSoftwareEngineer)))
            {
                isReadOnly = true;
                query = query.Where(x => x.ProjectMembers.Any(x => x.MemberId == currentUserService.UserId));
            }

            if (status > 0)
            {
                var projectStatus = (ProjectStatus)status;

                query = query.Where(x => x.Status == projectStatus);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.Name.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "name":
                        query = ascending
                            ? query.OrderBy(u => u.Name)
                            : query.OrderByDescending(u => u.Name);
                        break;
                    case "startDate":
                        query = ascending
                            ? query.OrderBy(u => u.StartDate)
                            : query.OrderByDescending(u => u.StartDate);
                        break;
                    case "endDate":
                        query = ascending
                            ? query.OrderBy(u => u.EndDate)
                            : query.OrderByDescending(u => u.EndDate);
                        break;
                    // Add other sort fields as needed
                    default:
                        query = ascending
                            ? query.OrderBy(u => u.Id)
                            : query.OrderByDescending(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sorting by full name
                query = query.OrderBy(u => u.Name);
            }

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => new ProjectDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            SelectedManagementPlatform = new DropDownDTO() { Id = (int)p.ManagementPlatform },
                            StartDate = p.StartDate,
                            StartDateText = String.Format("{0:MM/dd/yyyy}", p.StartDate),
                            EndDate = p.EndDate,
                            EndDateText = String.Format("{0:MM/dd/yyyy}", p.EndDate),
                            SelectedManager = new UserDropDownDTO() { Id = p.ManagerId },
                            ManagerName = context.ApplicationUsers.FirstOrDefault(x => x.Id == p.ManagerId).FullName,
                            StatusName = EnumHelper.GetEnumDescription(p.Status),
                            ClientName = p.ClientName,
                            ClientAddress = p.ClientAddress,
                            ClientPhone = p.ClientPhone,
                            ClientEmail = p.ClientEmail,
                            SelectedStatus = new DropDownDTO() { Id = (int)p.Status }
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<ProjectDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize,
                IsReadOnly = isReadOnly
            };

            return newResult;
        }

        public async Task<PaginatedResultDTO<ProjectDTO>> GetMyManageProjectsAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true)
        {

            var query = context.Projects.Where(x => x.IsActive == true && x.ManagerId == currentUserService.UserId);

            if (status > 0)
            {
                var projectStatus = (ProjectStatus)status;

                query = query.Where(x => x.Status == projectStatus);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.Name.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "name":
                        query = ascending
                            ? query.OrderBy(u => u.Name)
                            : query.OrderByDescending(u => u.Name);
                        break;
                    case "startDate":
                        query = ascending
                            ? query.OrderBy(u => u.StartDate)
                            : query.OrderByDescending(u => u.StartDate);
                        break;
                    case "endDate":
                        query = ascending
                            ? query.OrderBy(u => u.EndDate)
                            : query.OrderByDescending(u => u.EndDate);
                        break;
                    // Add other sort fields as needed
                    default:
                        query = ascending
                            ? query.OrderBy(u => u.Id)
                            : query.OrderByDescending(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sorting by full name
                query = query.OrderBy(u => u.Name);
            }

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => new ProjectDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            SelectedManagementPlatform = new DropDownDTO() { Id = (int)p.ManagementPlatform },
                            StartDate = p.StartDate,
                            StartDateText = String.Format("{0:MM/dd/yyyy}", p.StartDate),
                            EndDate = p.EndDate,
                            EndDateText = String.Format("{0:MM/dd/yyyy}", p.EndDate),
                            SelectedManager = new UserDropDownDTO() { Id = p.ManagerId },
                            ManagerName = context.ApplicationUsers.FirstOrDefault(x => x.Id == p.ManagerId).FullName,
                            StatusName = EnumHelper.GetEnumDescription(p.Status),
                            ClientName = p.ClientName,
                            ClientAddress = p.ClientAddress,
                            ClientPhone = p.ClientPhone,
                            ClientEmail = p.ClientEmail,
                            SelectedStatus = new DropDownDTO() { Id = (int)p.Status }
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<ProjectDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<PaginatedResultDTO<ProjectDTO>> GetMyAssignProjectsAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true)
        {

            var query = context.ProjectMembers
                .Where(x => x.IsActive && x.MemberId == currentUserService.UserId)
                .Select(p => p.Project);

            if (status > 0)
            {
                var projectStatus = (ProjectStatus)status;

                query = query.Where(x => x.Status == projectStatus);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.Name.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "name":
                        query = ascending
                            ? query.OrderBy(u => u.Name)
                            : query.OrderByDescending(u => u.Name);
                        break;
                    case "startDate":
                        query = ascending
                            ? query.OrderBy(u => u.StartDate)
                            : query.OrderByDescending(u => u.StartDate);
                        break;
                    case "endDate":
                        query = ascending
                            ? query.OrderBy(u => u.EndDate)
                            : query.OrderByDescending(u => u.EndDate);
                        break;
                    // Add other sort fields as needed
                    default:
                        query = ascending
                            ? query.OrderBy(u => u.Id)
                            : query.OrderByDescending(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sorting by full name
                query = query.OrderBy(u => u.Name);
            }

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => new ProjectDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            SelectedManagementPlatform = new DropDownDTO() { Id = (int)p.ManagementPlatform },
                            StartDate = p.StartDate,
                            StartDateText = String.Format("{0:MM/dd/yyyy}", p.StartDate),
                            EndDate = p.EndDate,
                            EndDateText = String.Format("{0:MM/dd/yyyy}", p.EndDate),
                            SelectedManager = new UserDropDownDTO() { Id = p.ManagerId },
                            ManagerName = context.ApplicationUsers.FirstOrDefault(x => x.Id == p.ManagerId).FullName,
                            StatusName = EnumHelper.GetEnumDescription(p.Status),
                            ClientName = p.ClientName,
                            ClientAddress = p.ClientAddress,
                            ClientPhone = p.ClientPhone,
                            ClientEmail = p.ClientEmail,
                            SelectedStatus = new DropDownDTO() { Id = (int)p.Status }
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<ProjectDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<List<ProjectDocumentDTO>> GetProjectDocuments(int projectId)
        {
            var projectDocuments = await context.ProjectDocuments
                .Where(x => x.ProjectId == projectId && x.IsActive)
                .Select(x => new ProjectDocumentDTO
                {
                    Id = x.Id,
                    OriginalFileName = x.OriginalFileName,
                    SavedFileName = x.SavedFileName,
                    SavedPath = x.SavedPath,
                }).ToListAsync();

            return projectDocuments;
        }

        public async Task<List<ProjectMemberDTO>> GetProjectMembers(int projectId)
        {
            var projectMembers = await context.ProjectMembers
                .Where(x => x.ProjectId == projectId && x.IsActive)
                .Select(x => new ProjectMemberDTO
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    MemberId = x.MemberId,
                    MemberName = x.Member.FullName,
                    RoleId = x.RoleId,
                    RoleName = x.Role.Name,
                    AllocatedDate = x.CreatedDate.ToString("MM/dd/yyyy"),
                    DeAllocatedDate = x.IsActive ? "Still Active" : x.UpdateDate.Value.ToString("MM/dd/yyyy")
                }).ToListAsync();

            return projectMembers;
        }

        public async Task<GeneralResponseDTO> SaveProject(ProjectDTO projectDto)
        {
            try
            {
                var project = await context.Projects.FindAsync(projectDto.Id);

                if (project is null)
                {
                    project = new Project
                    {
                        Name = projectDto.Name,
                        Description = projectDto.Description,
                        ManagementPlatform = (ProjectManagementPlatform)projectDto.SelectedManagementPlatform.Id,
                        StartDate = projectDto.StartDate,
                        EndDate = projectDto.EndDate,
                        ManagerId = projectDto.SelectedManager.Id,
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.UtcNow,
                        Status = (ProjectStatus)projectDto.SelectedStatus.Id,
                        UpdatedByUserId = currentUserService.UserId,
                        ClientName = projectDto.ClientName,
                        ClientAddress = projectDto.ClientAddress,
                        ClientPhone = projectDto.ClientPhone,
                        ClientEmail = projectDto.ClientEmail,
                        UpdateDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    context.Projects.Add(project);
                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO
                    {
                        Flag = true,
                        Message = "Project has been created successfully."
                    };
                }
                else
                {
                    project.Name = projectDto.Name;
                    project.Description = projectDto.Description;
                    project.ManagementPlatform = (ProjectManagementPlatform)projectDto.SelectedManagementPlatform.Id;
                    project.StartDate = projectDto.StartDate;
                    project.EndDate = projectDto.EndDate;
                    project.Status = (ProjectStatus)projectDto.SelectedStatus.Id;
                    project.ManagerId = projectDto.SelectedManager.Id;
                    project.ClientName = projectDto.ClientName;
                    project.ClientAddress = projectDto.ClientAddress;
                    project.ClientPhone = projectDto.ClientPhone;
                    project.ClientEmail = projectDto.ClientEmail;
                    project.UpdatedByUserId = currentUserService.UserId;
                    project.UpdateDate = DateTime.UtcNow;

                    context.Projects.Update(project);
                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO
                    {
                        Flag = true,
                        Message = "Project has been updated successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving project");
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the project."
                };
            }

        }

        public async Task<ProjectDTO> GetProjectById(int projectId)
        {
            var project = await context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return new ProjectDTO();
            }

            var projectDto = new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                SelectedManagementPlatform = new DropDownDTO() { Id = (int)project.ManagementPlatform },
                StartDate = project.StartDate,
                StartDateText = String.Format("{0:MM/dd/yyyy}", project.StartDate),
                EndDate = project.EndDate,
                EndDateText = String.Format("{0:MM/dd/yyyy}", project.EndDate),
                SelectedManager = new UserDropDownDTO() { Id = project.ManagerId },
                ManagerName = context.ApplicationUsers.FirstOrDefault(x => x.Id == project.ManagerId).FullName,
                StatusName = EnumHelper.GetEnumDescription(project.Status),
                ClientName = project.ClientName,
                ClientAddress = project.ClientAddress,
                ClientPhone = project.ClientPhone,
                ClientEmail = project.ClientEmail,
                SelectedStatus = new DropDownDTO() { Id = (int)project.Status }
            };

            return projectDto;
        }
        public async Task<List<DropDownDTO>> GetMyAssignedProjects()
        {
            var assignedProjects = await context
                .ProjectMembers
                .Where(x => x.IsActive && x.MemberId == currentUserService.UserId)
                .Select(x => new DropDownDTO()
                {
                    Id = x.ProjectId,
                    Name = x.Project.Name
                }).ToListAsync(CancellationToken.None);

            return assignedProjects;
        }
    }
}
