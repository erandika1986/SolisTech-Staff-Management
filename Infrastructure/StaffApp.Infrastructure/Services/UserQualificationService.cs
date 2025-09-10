using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.UserQualification;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;

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
                    DocumentNameId = dto.SelectedDocumentName.Id,
                    OtherName = string.IsNullOrEmpty(dto.OtherName) ? null : dto.OtherName,
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

        //public async Task<List<UserQualificationDTO>> GetAllAsync(string userId)
        //{
        //    var qualifications = await context.UserQualificationDocuments
        //        .Where(q => q.UserId == userId && q.IsActive)
        //        .Select(q => new UserQualificationDTO
        //        {
        //            Id = q.Id,
        //            UserId = q.UserId,
        //            QualificationName = q.QualificationName,
        //            SelectedDocumentName = new DropDownDTO() { Id = q.DocumentNameId },
        //            OtherDocumentName = !string.IsNullOrEmpty(q.OtherName) ? q.OtherName : null,
        //            OriginalFileName = q.OriginalFileName,
        //            SaveFileName = q.SaveFileName,
        //            Path = q.Path
        //        }).ToListAsync();


        //    return qualifications;
        //}

        //public async Task<UserQualificationDTO> GetByIdAsync(int id)
        //{
        //    var qualification = await context.UserQualificationDocuments
        //        .Where(q => q.Id == id && q.IsActive)
        //        .Select(q => new UserQualificationDTO
        //        {
        //            Id = q.Id,
        //            UserId = q.UserId,
        //            QualificationName = q.QualificationName,
        //            SelectedDocumentName = new DropDownDTO() { Id = q.DocumentNameId },
        //            OtherDocumentName = !string.IsNullOrEmpty(q.OtherName) ? q.OtherName : null,
        //            OriginalFileName = q.OriginalFileName,
        //            SaveFileName = q.SaveFileName,
        //            Path = q.Path
        //        }).FirstOrDefaultAsync();

        //    return qualification;
        //}

        //public async Task<GeneralResponseDTO> UpdateAsync(UserQualificationDTO dto)
        //{
        //    try
        //    {
        //        var userQualification = await context.UserQualificationDocuments.FirstOrDefaultAsync(q => q.Id == dto.Id && q.IsActive);

        //        if (userQualification == null)
        //        {
        //            return new GeneralResponseDTO
        //            {
        //                Flag = false,
        //                Message = "Qualification not found."
        //            };
        //        }

        //        if (dto.Files != null && dto.Files.Any())
        //        {
        //            await UploadQualificationFiles(dto.Files, userQualification);
        //        }

        //        userQualification.QualificationName = dto.QualificationName;
        //        userQualification.DocumentNameId = dto.SelectedDocumentName.Id;
        //        userQualification.QualificationName = string.IsNullOrEmpty(dto.OtherDocumentName) ? null : dto.OtherDocumentName;

        //        context.UserQualificationDocuments.Update(userQualification);
        //        await context.SaveChangesAsync(CancellationToken.None);

        //        return new GeneralResponseDTO
        //        {
        //            Flag = true,
        //            Message = "Qualification updated successfully."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error updating qualification with Id {Id}", dto.Id);
        //        return new GeneralResponseDTO
        //        {
        //            Flag = false,
        //            Message = "An error occurred while updating the qualification."
        //        };
        //    }
        //}

        public List<DocumentCategoryDTO> GetDocumentCategories()
        {
            var response = Enum.GetValues(typeof(EmployeeDocumentCategory))
                .Cast<EmployeeDocumentCategory>()
                .Select(e => new DocumentCategoryDTO
                {
                    Id = (int)e,
                    Name = EnumHelper.GetEnumDescription(e),
                    NoOfDocuments = context.UserQualificationDocuments.Count(d => d.UserId == currentUserService.UserId && d.DocumentName.EmployeeDocumentCategory == e)
                }).ToList();


            return response;
        }

        public async Task<List<DropDownDTO>> GetDocumentsByCategory(int categoryId)
        {
            var categories = await context.DocumentNames.Where(x => x.EmployeeDocumentCategory == (EmployeeDocumentCategory)categoryId)
                .Select(x => new DropDownDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return categories;
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
                userQualificationDocument.FileSize = uploadedFile.Size;
                userQualificationDocument.FileType = Path.GetExtension(uploadedFile.Name).TrimStart('.');

                return userQualificationDocument;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading qualification files for user {UserId}", userQualificationDocument.UserId);
                throw;
            }

        }

        public async Task<List<EmployeeDocumentCategoryContainerDTO>> GetEmployeeDocumentsByUserId(string userId)
        {
            var result = new List<EmployeeDocumentCategoryContainerDTO>();

            foreach (EmployeeDocumentCategory category in (EmployeeDocumentCategory[])Enum.GetValues(typeof(EmployeeDocumentCategory)))
            {
                var container = new EmployeeDocumentCategoryContainerDTO
                {
                    DocumentCategory = new DocumentCategoryDTO
                    {
                        Id = (int)category,
                        Name = EnumHelper.GetEnumDescription(category),
                        NoOfDocuments = context.UserQualificationDocuments.Count(d => d.UserId == currentUserService.UserId && d.DocumentName.EmployeeDocumentCategory == category)
                    },
                    EmployeeDocuments = await context.UserQualificationDocuments
                        .Where(d => d.UserId == userId && d.IsActive && d.DocumentName.EmployeeDocumentCategory == category)
                        .Select(d => new EmployeeDocumentDTO
                        {
                            DocumentCategory = EnumHelper.GetEnumDescription(d.DocumentName.EmployeeDocumentCategory),
                            UserId = d.UserId,
                            OriginalFileName = d.DocumentName.Name == "Other" ? $"{d.DocumentName.Name}({d.OtherName}) - {d.OriginalFileName}" : $"{d.DocumentName.Name}-{d.OriginalFileName}",
                            Path = d.Path,
                            FileSize = d.FileSize,
                            FileType = Path.GetExtension(d.OriginalFileName),
                            Id = d.Id,
                            SaveFileName = d.SaveFileName,
                            UploadDate = d.CreatedDate
                        }).ToListAsync()
                };

                result.Add(container);
            }

            return result;
        }

        public Task<List<UserQualificationDTO>> GetAllAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserQualificationDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponseDTO> UpdateAsync(UserQualificationDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
