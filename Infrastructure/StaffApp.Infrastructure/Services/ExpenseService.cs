using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class ExpenseService(IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        IConfiguration configuration,
        ILogger<IExpenseService> logger) : IExpenseService
    {
        public async Task<GeneralResponseDTO> DeleteExpense(int expenseId)
        {
            try
            {
                var expense = await context.Expenses
                    .Include(e => e.ExpenseSupportAttachments)
                    .FirstOrDefaultAsync(e => e.Id == expenseId);

                if (expense == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Expense not found"
                    };
                }

                expense.IsActive = false;
                expense.UpdatedByUserId = currentUserService.UserId;
                expense.UpdateDate = DateTime.Now;

                context.Expenses.Update(expense);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Expense deleted successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting expense with ID {ExpenseId}", expenseId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while deleting the expense"
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteExpenseSupportDocument(int expenseId, int supportDocumentId)
        {
            try
            {
                var supportDocument = await context.ExpenseSupportAttachments.FirstOrDefaultAsync(s => s.Id == supportDocumentId && s.ExpenseId == expenseId);
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

                context.ExpenseSupportAttachments.Update(supportDocument);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Support document deleted successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting support document with ID {SupportDocumentId} for expense ID {ExpenseId}", supportDocumentId, expenseId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while deleting the support document"
                };
            }
        }

        public async Task<PaginatedResultDTO<ExpenseDTO>> GetAllExpensesAsync(int pageNumber, int pageSize, DateTime startDate, DateTime endDate, int expenseTypeId)
        {
            var query = context.Expenses
                .Include(e => e.ExpenseType)
                .Include(e => e.ExpenseSupportAttachments)
                .Where(e => e.Date >= startDate && e.Date <= endDate && e.IsActive);

            if (expenseTypeId > 0)
            {
                query = query.Where(e => e.ExpenseTypeId == expenseTypeId);
            }

            var totalCount = await query.CountAsync();

            var expenses = await query
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ExpenseDTO
                {
                    Id = e.Id,
                    Date = e.Date,
                    Amount = (double)e.Amount,
                    DateName = e.Date.ToString("MM/dd/yyyy"),
                    Notes = e.Notes,
                    ExpenseType = new DropDownDTO() { Id = e.ExpenseTypeId },
                    ExpenseTypeName = e.ExpenseType.Name,
                    CreatedByUser = e.CreatedByUserId != null ? context.ApplicationUsers.FirstOrDefault(u => u.Id == e.CreatedByUserId).FullName : "N/A",
                    CreatedOn = e.CreatedDate.ToString("MM/dd/yyyy"),
                    UpdatedByUser = e.UpdatedByUserId != null ? context.ApplicationUsers.FirstOrDefault(u => u.Id == e.UpdatedByUserId).FullName : "N/A",
                    CompanyYear = new DropDownDTO() { Id = e.CompanyYearId, Name = e.CompanyYear.Year.ToString() },
                    Month = new DropDownDTO() { Id = (int)e.Month, Name = EnumHelper.GetEnumDescription(e.Month) },
                    UpdatedOn = e.UpdateDate.Value.ToString("MM/dd/yyyy"),
                    SavedSupportFiles = e.ExpenseSupportAttachments.Select(a => new SupportAttachmentDTO
                    {
                        Id = a.SupportAttachmentId,
                        OriginalFileName = a.SupportAttachment.OriginalFileName,
                        SavedFileName = a.SupportAttachment.SavedFileName,
                        SaveFileURL = a.SupportAttachment.SaveFileURL,
                    }).ToList()
                })
                .ToListAsync();

            var newResult = new PaginatedResultDTO<ExpenseDTO>
            {
                Items = expenses,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize,
                IsReadOnly = true
            };

            return newResult;
        }

        public async Task<GeneralResponseDTO> SaveExpense(ExpenseDTO expenseDto)
        {
            try
            {
                Expense expense = await context.Expenses
                    .FirstOrDefaultAsync(e => e.Id == expenseDto.Id);

                if (expense is null)
                {
                    expense = new Expense
                    {
                        Date = expenseDto.Date,
                        Amount = (decimal)expenseDto.Amount,
                        Notes = expenseDto.Notes,
                        ExpenseTypeId = expenseDto.ExpenseType.Id,
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        UpdatedByUserId = currentUserService.UserId,
                        IsActive = true,
                        CompanyYearId = expenseDto.CompanyYear.Id,
                        Month = (Month)expenseDto.Month.Id,
                        EmployeeShare = (decimal?)expenseDto.EmployeeShare,
                        CompanyShare = (decimal?)expenseDto.CompanyShare,
                    };

                    context.Expenses.Add(expense);
                    await context.SaveChangesAsync(CancellationToken.None);

                    await UploadFiles(expenseDto, expense);


                }
                else
                {
                    expense.CompanyYearId = expenseDto.CompanyYear.Id;
                    expense.Month = (Month)expenseDto.Month.Id;
                    expense.EmployeeShare = (decimal?)expenseDto.EmployeeShare;
                    expense.CompanyShare = (decimal?)expenseDto.CompanyShare;
                    expense.Amount = (decimal)expenseDto.Amount;
                    expense.Date = expenseDto.Date;
                    expense.Notes = expenseDto.Notes;
                    expense.ExpenseTypeId = expenseDto.ExpenseType.Id;
                    expense.UpdatedByUserId = currentUserService.UserId;
                    expense.UpdateDate = DateTime.Now;

                    await UploadFiles(expenseDto, expense);

                    context.Expenses.Update(expense);

                    await context.SaveChangesAsync(CancellationToken.None);
                }



                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Expense saved successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving expense with ID {ExpenseId}", expenseDto.Id);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while saving the expense: " + ex.Message
                };
            }
        }

        public async Task<ExpenseMasterDataDTO> GetExpenseMasterData(bool hasDefaultValue = false)
        {
            var response = new ExpenseMasterDataDTO();

            if (hasDefaultValue)
                response.ExpenseTypes.Add(new DropDownDTO() { Id = 0, Name = "All" });

            var expenseTypes = await context.ExpenseTypes
                .OrderBy(et => et.Name)
                .Select(et => new DropDownDTO
                {
                    Id = et.Id,
                    Name = et.Name
                })
                .ToListAsync();

            response.ExpenseTypes.AddRange
                (expenseTypes);

            var companyYears = await context.CompanyYears
                .OrderByDescending(cy => cy.Year)
                .Select(cy => new DropDownDTO
                {
                    Id = cy.Id,
                    Name = cy.Year.ToString()
                })
                .ToListAsync();

            response.CompanyYears.AddRange(companyYears);

            response.Months = EnumHelper.GetDropDownList<Month>();

            return response;
        }

        private async Task<Expense> UploadFiles(
            ExpenseDTO expenseDTO,
            Expense expense)
        {
            string supportDocumentPath = configuration["FileSavePaths:SupportDocumentPath"];
            if (!Directory.Exists(supportDocumentPath))
            {
                Directory.CreateDirectory(supportDocumentPath);
            }


            long maxFileSize = 10 * 1024 * 1024;

            for (int i = 0; i < expenseDTO.Files.Count; i++)
            {
                var extension = Path.GetExtension(expenseDTO.Files[i].Name);

                var fileName = $"{Path.GetFileNameWithoutExtension(expenseDTO.Files[i].Name)}{extension}";
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(supportDocumentPath, uniqueFileName);

                await using var stream = expenseDTO.Files[i].OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadedFileUrl = await azureBlobService
                    .UploadFileAsync(memoryStream, uniqueFileName, expenseDTO.Files[i].ContentType, ApplicationConstants.AzureBlobStorageName);

                expense.ExpenseSupportAttachments.Add(new ExpenseSupportAttachment()
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
                context.Expenses.Update(expense);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            return expense;
        }

        public async Task<ExpenseDTO> GetExpenseByIdAsync(int expenseId)
        {
            var expense = await context.Expenses
                .Include(e => e.ExpenseType)
                .Include(e => e.ExpenseSupportAttachments)
                .ThenInclude(a => a.SupportAttachment)
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.IsActive);

            var expenseDTO = new ExpenseDTO
            {
                Id = expense.Id,
                Date = expense.Date,
                Amount = (double)expense.Amount,
                Notes = expense.Notes,
                ExpenseType = new DropDownDTO() { Id = expense.ExpenseTypeId },
                ExpenseTypeName = expense.ExpenseType.Name,
                CompanyYear = new DropDownDTO() { Id = expense.CompanyYearId, Name = expense.CompanyYear.Year.ToString() },
                Month = new DropDownDTO() { Id = (int)expense.Month, Name = EnumHelper.GetEnumDescription(expense.Month) },
                EmployeeShare = (double?)expense.EmployeeShare,
                CompanyShare = (double?)expense.CompanyShare,
                SavedSupportFiles = expense.ExpenseSupportAttachments.Select(a => new SupportAttachmentDTO
                {
                    Id = a.SupportAttachmentId,
                    OriginalFileName = a.SupportAttachment.OriginalFileName,
                    SavedFileName = a.SupportAttachment.SavedFileName,
                    SaveFileURL = a.SupportAttachment.SaveFileURL,
                }).ToList()
            };

            return expenseDTO;
        }
    }
}

