using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class IncomeService(
        IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        IConfiguration configuration,
        ILogger<IExpenseService> logger) : IIncomeService
    {
        public async Task<GeneralResponseDTO> DeleteIncome(int incomeId)
        {
            try
            {
                var income = await context.Incomes
                    .Include(e => e.IncomeSupportAttachments)
                    .FirstOrDefaultAsync(e => e.Id == incomeId);

                if (income == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Income not found"
                    };
                }

                income.IsActive = false;
                income.UpdatedByUserId = currentUserService.UserId;
                income.UpdateDate = DateTime.Now;

                context.Incomes.Update(income);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Income deleted successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting income with ID {incomeId}", incomeId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while deleting the expense"
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteIncomeSupportDocument(int incomeId, int supportDocumentId)
        {
            try
            {
                var supportDocument = await context.IncomeSupportAttachments.FirstOrDefaultAsync(s => s.Id == supportDocumentId && s.IncomeId == incomeId);
                if (supportDocument == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Support document not found"
                    };
                }

                supportDocument.IsActive = false;
                supportDocument.UpdatedByUserId = currentUserService.UserId;
                supportDocument.UpdateDate = DateTime.UtcNow;

                context.IncomeSupportAttachments.Update(supportDocument);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Support document deleted successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting support document with ID {SupportDocumentId} for expense ID {IncomeId}", supportDocumentId, incomeId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while deleting the support document"
                };
            }
        }

        public async Task<PaginatedResultDTO<IncomeDTO>> GetAllIncomeAsync(int pageNumber, int pageSize, DateTime startDate, DateTime endDate, int incomeTypeId)
        {
            var query = context.Incomes
                .Include(e => e.IncomeType)
                .Include(e => e.IncomeSupportAttachments)
                .Where(e => e.Date >= startDate && e.Date <= endDate && e.IsActive);

            if (incomeTypeId > 0)
            {
                query = query.Where(e => e.IncomeTypeId == incomeTypeId);
            }

            var totalCount = await query.CountAsync();

            var incomes = await query
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new IncomeDTO
                {
                    Id = e.Id,
                    Date = e.Date,
                    Amount = (double)e.Amount,
                    DateName = e.Date.ToString("MM/dd/yyyy"),
                    Notes = e.Notes,
                    IncomeType = new DropDownDTO() { Id = e.IncomeTypeId },
                    IncomeTypeName = e.IncomeType.Name,
                    CreatedByUser = e.CreatedByUserId != null ? context.ApplicationUsers.FirstOrDefault(u => u.Id == e.CreatedByUserId).FullName : "N/A",
                    CreatedOn = e.CreatedDate.ToString("MM/dd/yyyy"),
                    UpdatedByUser = e.UpdatedByUserId != null ? context.ApplicationUsers.FirstOrDefault(u => u.Id == e.UpdatedByUserId).FullName : "N/A",
                    UpdatedOn = e.UpdateDate.Value.ToString("MM/dd/yyyy"),
                    SavedSupportFiles = e.IncomeSupportAttachments.Select(a => new SupportAttachmentDTO
                    {
                        Id = a.SupportAttachmentId,
                        OriginalFileName = a.SupportAttachment.OriginalFileName,
                        SavedFileName = a.SupportAttachment.SavedFileName,
                        SaveFileURL = a.SupportAttachment.SaveFileURL,
                    }).ToList()
                })
                .ToListAsync();

            var newResult = new PaginatedResultDTO<IncomeDTO>
            {
                Items = incomes,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize,
                IsReadOnly = true
            };

            return newResult;
        }

        public async Task<IncomeDTO> GetIncomeByIdAsync(int incomeId)
        {
            var income = await context.Incomes
                .Include(e => e.IncomeType)
                .Include(e => e.IncomeSupportAttachments)
                .ThenInclude(a => a.SupportAttachment)
                .FirstOrDefaultAsync(e => e.Id == incomeId && e.IsActive);

            var expenseDTO = new IncomeDTO
            {
                Id = income.Id,
                Date = income.Date,
                Amount = (double)income.Amount,
                Notes = income.Notes,
                IncomeType = new DropDownDTO() { Id = income.IncomeTypeId },
                IncomeTypeName = income.IncomeType.Name,
                SavedSupportFiles = income.IncomeSupportAttachments.Select(a => new SupportAttachmentDTO
                {
                    Id = a.SupportAttachmentId,
                    OriginalFileName = a.SupportAttachment.OriginalFileName,
                    SavedFileName = a.SupportAttachment.SavedFileName,
                    SaveFileURL = a.SupportAttachment.SaveFileURL,
                }).ToList()
            };

            return expenseDTO;
        }

        public async Task<List<DropDownDTO>> GetIncomeTypes(bool hasDefaultValue = false)
        {
            var response = new List<DropDownDTO>();

            if (hasDefaultValue)
                response.Add(new DropDownDTO() { Id = 0, Name = "All" });

            var expenseTypes = await context.IncomeTypes
                .OrderBy(et => et.Name)
                .Select(et => new DropDownDTO
                {
                    Id = et.Id,
                    Name = et.Name
                })
                .ToListAsync();

            response.AddRange
                (expenseTypes);

            return response;
        }

        public async Task<GeneralResponseDTO> SaveIncome(IncomeDTO incomeDto)
        {
            try
            {
                Income income = await context.Incomes
                    .FirstOrDefaultAsync(e => e.Id == incomeDto.Id);

                if (income is null)
                {
                    income = new Income
                    {
                        Date = incomeDto.Date,
                        Amount = (decimal)incomeDto.Amount,
                        Notes = incomeDto.Notes,
                        IncomeTypeId = incomeDto.IncomeType.Id,
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        UpdatedByUserId = currentUserService.UserId,
                        IsActive = true

                    };

                    context.Incomes.Add(income);
                    await context.SaveChangesAsync(CancellationToken.None);

                    await UploadFiles(incomeDto, income);


                }
                else
                {
                    income.Amount = (decimal)incomeDto.Amount;
                    income.Date = incomeDto.Date;
                    income.Notes = incomeDto.Notes;
                    income.IncomeTypeId = incomeDto.IncomeType.Id;
                    income.UpdatedByUserId = currentUserService.UserId;
                    income.UpdateDate = DateTime.Now;

                    await UploadFiles(incomeDto, income);

                    context.Incomes.Update(income);

                    await context.SaveChangesAsync(CancellationToken.None);
                }



                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Income saved successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving income with ID {IncomeId}", incomeDto.Id);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the income: " + ex.Message
                };
            }
        }

        private async Task<Income> UploadFiles(
            IncomeDTO incomeDTO,
            Income income)
        {
            string supportDocumentPath = configuration["FileSavePaths:SupportDocumentPath"];
            if (!Directory.Exists(supportDocumentPath))
            {
                Directory.CreateDirectory(supportDocumentPath);
            }


            long maxFileSize = 10 * 1024 * 1024;

            for (int i = 0; i < incomeDTO.Files.Count; i++)
            {
                var extension = Path.GetExtension(incomeDTO.Files[i].Name);

                var fileName = $"{Path.GetFileNameWithoutExtension(incomeDTO.Files[i].Name)}{extension}";
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(supportDocumentPath, uniqueFileName);

                await using var stream = incomeDTO.Files[i].OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadedFileUrl = await azureBlobService
                    .UploadFileAsync(memoryStream, uniqueFileName, incomeDTO.Files[i].ContentType, ApplicationConstants.AzureBlobStorageName);

                income.IncomeSupportAttachments.Add(new IncomeSupportAttachment()
                {
                    SupportAttachment = new SupportAttachment
                    {
                        OriginalFileName = fileName,
                        SavedFileName = uniqueFileName,
                        SaveFileURL = uploadedFileUrl
                    },
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    UpdateDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                });
                context.Incomes.Update(income);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            return income;
        }
    }
}
