using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.UserQualification;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class UserQualificationService(
        IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        IConfiguration configuration,
        ILogger<IUserQualificationService> logger) : IUserQualificationService
    {
        public async Task<GeneralResponseDTO> CreateAsync(UserQualificationDTO dto)
        {
            try
            {
                var userQualification = new UserQualificationDocument
                {
                    UserId = dto.UserId,
                    QualificationName = dto.QualificationName,
                    DocumentName = dto.DocumentName,
                    OriginalFileName = dto.OriginalFileName,
                    SaveFileName = dto.SaveFileName,
                    Path = dto.Path,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.UtcNow,
                    IsActive = true
                };

                if (dto.Files != null && dto.Files.Any())
                {
                    await UploadQualificationFiles(dto.Files, userQualification);
                }

                context.UserQualificationDocuments.Add(userQualification);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Qualification added successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding qualification for user {UserId}", dto.UserId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while adding the qualification."
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteAsync(int id)
        {
            try
            {
                var qualification = await context.UserQualificationDocuments.FirstOrDefaultAsync(q => q.Id == id);

                if (qualification == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Qualification not found."
                    };
                }

                qualification.IsActive = false;
                qualification.UpdatedByUserId = currentUserService.UserId;
                qualification.UpdateDate = DateTime.UtcNow;

                context.UserQualificationDocuments.Update(qualification);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Qualification deleted successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting qualification with Id {Id}", id);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while deleting the qualification."
                };
            }
        }

        public async Task<List<UserQualificationDTO>> GetAllAsync(string userId)
        {
            var qualifications = await context.UserQualificationDocuments
                .Where(q => q.UserId == userId && q.IsActive)
                .Select(q => new UserQualificationDTO
                {
                    Id = q.Id,
                    UserId = q.UserId,
                    QualificationName = q.QualificationName,
                    DocumentName = q.DocumentName,
                    OriginalFileName = q.OriginalFileName,
                    SaveFileName = q.SaveFileName,
                    Path = q.Path
                }).ToListAsync();


            return qualifications;
        }

        public async Task<UserQualificationDTO> GetByIdAsync(int id)
        {
            var qualification = await context.UserQualificationDocuments
                .Where(q => q.Id == id && q.IsActive)
                .Select(q => new UserQualificationDTO
                {
                    Id = q.Id,
                    UserId = q.UserId,
                    QualificationName = q.QualificationName,
                    DocumentName = q.DocumentName,
                    OriginalFileName = q.OriginalFileName,
                    SaveFileName = q.SaveFileName,
                    Path = q.Path
                }).FirstOrDefaultAsync();

            return qualification;
        }

        public async Task<GeneralResponseDTO> UpdateAsync(UserQualificationDTO dto)
        {
            try
            {
                var userQualification = await context.UserQualificationDocuments.FirstOrDefaultAsync(q => q.Id == dto.Id && q.IsActive);

                if (userQualification == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Qualification not found."
                    };
                }

                if (dto.Files != null && dto.Files.Any())
                {
                    await UploadQualificationFiles(dto.Files, userQualification);
                }

                userQualification.QualificationName = dto.QualificationName;
                userQualification.DocumentName = dto.DocumentName;

                context.UserQualificationDocuments.Update(userQualification);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Qualification updated successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating qualification with Id {Id}", dto.Id);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while updating the qualification."
                };
            }
        }

        private async Task<UserQualificationDocument> UploadQualificationFiles(List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> files, UserQualificationDocument userQualificationDocument)
        {
            try
            {
                string supportDocumentPath = configuration["FileSavePaths:SupportDocumentPath"];
                if (!Directory.Exists(supportDocumentPath))
                {
                    Directory.CreateDirectory(supportDocumentPath);
                }

                long maxFileSize = 10 * 1024 * 1024;

                var uploadedFile = files.FirstOrDefault();

                var extension = Path.GetExtension(uploadedFile.Name);

                var fileName = $"{Path.GetFileNameWithoutExtension(uploadedFile.Name)}{extension}";
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(supportDocumentPath, uniqueFileName);

                await using var stream = uploadedFile.OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadedFileUrl = await azureBlobService
                    .UploadFileAsync(memoryStream, uniqueFileName, uploadedFile.ContentType, ApplicationConstants.AzureBlobStorageName);

                userQualificationDocument.OriginalFileName = fileName;
                userQualificationDocument.Path = uploadedFileUrl;
                userQualificationDocument.SaveFileName = uniqueFileName;

                return userQualificationDocument;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading qualification files for user {UserId}", userQualificationDocument.UserId);
                throw;
            }

        }
    }
}
