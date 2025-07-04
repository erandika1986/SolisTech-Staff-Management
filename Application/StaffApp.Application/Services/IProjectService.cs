﻿using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Project;

namespace StaffApp.Application.Services
{
    public interface IProjectService
    {
        Task<GeneralResponseDTO> SaveProject(ProjectDTO project);
        Task<GeneralResponseDTO> DeleteProject(int projectId);
        Task<PaginatedResultDTO<ProjectDTO>> GetAllProjectsAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true);
        Task<PaginatedResultDTO<ProjectDTO>> GetMyManageProjectsAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true);
        Task<PaginatedResultDTO<ProjectDTO>> GetMyAssignProjectsAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true);
        Task<List<UserDropDownDTO>> GetAvailableProjectMembers(int projectId);
        Task<GeneralResponseDTO> AddProjectMember(int Id, int projectId, string userId, string roleId, double hourlyRate);
        Task<GeneralResponseDTO> DeleteProjectMember(int projectMember);
        Task<List<ProjectMemberDTO>> GetProjectMembers(int projectId);
        Task<GeneralResponseDTO> AddProjectDocument(ProjectDocumentAttachmentDTO projectDocumentAttachment);
        Task<GeneralResponseDTO> DeleteProjectDocument(int projectDocumentId);
        Task<List<ProjectDocumentDTO>> GetProjectDocuments(int projectId);
        Task<ProjectDTO> GetProjectById(int projectId);
        Task<List<DropDownDTO>> GetMyAssignedProjects();
    }
}
