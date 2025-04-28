using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.Reflection;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace StaffApp.Infrastructure.Services
{
    public class UserSalaryService(IStaffAppDbContext context, IUserService userService, ICurrentUserService currentUserService, ILogger<IUserSalaryService> userSalaryServiceLogger) : IUserSalaryService
    {
        public async Task<GeneralResponseDTO> ApproveUserSalaryAsync(EmployeeSalaryDTO salary, string comment)
        {
            try
            {
                var employeeSalary = await context.EmployeeSalaries.FindAsync(salary.Id);

                if (employeeSalary == null)
                {
                    return new GeneralResponseDTO() { Flag = false, Message = "User salary not found." };
                }

                employeeSalary.Status = Domain.Enum.EmployeeSalaryStatus.Approved;
                employeeSalary.UpdatedByUserId = currentUserService.UserId;
                employeeSalary.UpdateDate = DateTime.Now;
                context.EmployeeSalaries.Update(employeeSalary);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "User salary successfully approved." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }

        }

        public async Task<GeneralResponseDTO> AskToReviseUserSalaryAsync(EmployeeSalaryDTO salary, string comment)
        {
            try
            {
                var employeeSalary = await context.EmployeeSalaries.FindAsync(salary.Id);

                if (employeeSalary == null)
                {
                    return new GeneralResponseDTO() { Flag = false, Message = "User salary not found." };
                }

                employeeSalary.Status = Domain.Enum.EmployeeSalaryStatus.SubmittedForRevised;
                employeeSalary.UpdatedByUserId = currentUserService.UserId;
                employeeSalary.UpdateDate = DateTime.Now;
                context.EmployeeSalaries.Update(employeeSalary);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "User salary not approved and asked for the revise." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<string> GenerateEstimateSalarySlip(EmployeeSalarySlipDTO salarySlip)
        {
            string filePath = string.Empty;

            try
            {
                var outPutDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

                var templatePath = System.IO.Path.Combine(outPutDirectory, @"WordTemplates\SalarySlipTemplate.docx");

                // Create a temporary file for the modified document
                string fileName = $"SalarySlip_{Guid.NewGuid().ToString()}.docx";
                string tempDocxPath = System.IO.Path.Combine(@"C:\WordDocuments\", fileName);
                string tempPdfPath = System.IO.Path.Combine(@"C:\WordDocuments\", System.IO.Path.ChangeExtension(fileName, ".pdf"));

                File.Copy(templatePath, tempDocxPath, true);

                // Modify the document using OpenXML
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(tempDocxPath, true))
                {
                    // Get the main document part
                    MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                    ReplaceText(mainPart, "CompanyName", salarySlip.CompanyName);
                    ReplaceText(mainPart, "CompanyAddress", salarySlip.CompanyAddress);
                    ReplaceText(mainPart, "CompanyTelephone ", salarySlip.CompanyPhone);
                    ReplaceText(mainPart, "CompanyEmail", salarySlip.CompanyEmail);
                    ReplaceText(mainPart, "YearAndMonth", $"{salarySlip.SalarySlipYear}-{salarySlip.SalarySlipMonth}");
                    ReplaceText(mainPart, "SLIPNumber", salarySlip.SalarySlipNumber);

                    ReplaceText(mainPart, "EmployeeNumber", salarySlip.EmployeeId);
                    ReplaceText(mainPart, "EmployeeName", salarySlip.EmployeeName);
                    ReplaceText(mainPart, "EmployeeDesignation", salarySlip.Designation);
                    ReplaceText(mainPart, "EmployeeDepartment", string.Empty);
                    ReplaceText(mainPart, "EmployeeJoinDate", salarySlip.JoinDate);

                    ReplaceText(mainPart, "PayPeriod", salarySlip.PayPeriod);
                    ReplaceText(mainPart, "PaymentDate", salarySlip.PayDate);
                    ReplaceText(mainPart, "PaymentMethod", salarySlip.PaymentMethod);
                    ReplaceText(mainPart, "DaysWorked", salarySlip.DaysWorked.ToString());
                    ReplaceText(mainPart, "LeaveTaken", salarySlip.LeaveTaken.ToString());

                    ReplaceText(mainPart, "BankName", salarySlip.BankName);
                    ReplaceText(mainPart, "AccountNumber", salarySlip.AccountNumber);
                    ReplaceText(mainPart, "BranchName", salarySlip.Branch);

                    ReplaceText(mainPart, "NetSalary", salarySlip.NetSalary.ToString("C"));

                    Document doc = mainPart.Document;
                    List<Table> tables = doc.Descendants<Table>().ToList();
                    if (tables.Count > 0)
                    {
                        Table firstTable = tables[8];

                        DocumentFormat.OpenXml.Wordprocessing.TableRow row = firstTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);

                        if (row != null)
                        {
                            // Fill values in specific cells
                            FillCellValue(row, 0, "John Doe");      // First column
                            FillCellValue(row, 1, "Employee123");   // Third column
                        }
                    }

                    // Save the changes
                    mainPart.Document.Save();
                }
                // Convert the modified document to PDF
                ConvertWordToPdf(tempDocxPath, tempPdfPath);

            }
            catch (Exception ex)
            {
                userSalaryServiceLogger.LogError(ex.ToString());
            }

            return filePath;
        }

        public async Task<PaginatedResultDTO<EmployeeSalaryBasicDTO>> GetAllUsersSalariesAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true)
        {
            var query = context.EmployeeSalaries.Where(x => x.User.IsActive == true);

            if (status > 0)
            {
                var salaryStatus = (EmployeeSalaryStatus)status;

                query = query.Where(x => x.Status == salaryStatus);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.User.FullName.Contains(searchString) ||
                    u.User.UserName.Contains(searchString) ||
                    u.User.PhoneNumber.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "fullName":
                        query = ascending
                            ? query.OrderBy(u => u.User.FullName)
                            : query.OrderByDescending(u => u.User.FullName);
                        break;
                    case "basicSalary":
                        query = ascending
                            ? query.OrderBy(u => u.BaseSalary)
                            : query.OrderByDescending(u => u.BaseSalary);
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
                query = query.OrderBy(u => u.User.FullName);
            }

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(salary => new EmployeeSalaryBasicDTO
                        {
                            Id = salary.Id,
                            BasicSalary = salary.BaseSalary.ToString("C"),
                            EffectiveFrom = salary.EffectiveFrom.ToString("yyyy-MM-dd"),
                            StatusText = EnumHelper.GetEnumDescription(salary.Status),
                            Status = salary.Status,
                            FullName = salary.User.FullName,
                            UserId = salary.User.Id,
                            UpdatedBy = context.ApplicationUsers.FirstOrDefault(x => x.Id == salary.UpdatedByUserId).FullName,
                            UpdatedOn = salary.UpdateDate.Value.ToString("yyyy-MM-dd")
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<EmployeeSalaryBasicDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<EmployeeSalarySlipDTO> GetEmployeeEstimateSalarySlip(string userId)
        {
            var response = new EmployeeSalarySlipDTO();

            var employee = await context.ApplicationUsers
                .Include(x => x.EmployeeBankAccounts)
                .Include(x => x.EmployeeSalaries)
                .ThenInclude(x => x.EmployeeSalaryAddons)
                .FirstOrDefaultAsync(x => x.Id == userId);



            var companyName = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyName);
            var companyAddress = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyAddress);
            var companyLogoUrl = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyLogoUrl);
            var companyEmail = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyEmail);
            var companyPhone = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyPhone);

            response.CompanyName = companyName is not null ? companyName.Value : string.Empty;
            response.LogoUrl = companyLogoUrl is not null ? companyLogoUrl.Value : string.Empty;
            response.CompanyAddress = companyAddress is not null ? companyAddress.Value : string.Empty;
            response.CompanyEmail = companyEmail is not null ? companyEmail.Value : string.Empty;
            response.CompanyPhone = companyPhone is not null ? companyPhone.Value : string.Empty;

            response.SalarySlipMonth = DateTime.Now.ToString("MMMM");
            response.SalarySlipYear = DateTime.Now.Year.ToString();
            response.SalarySlipNumber = "####";

            if (employee == null)
                return response;



            DateTime now = DateTime.Now;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var assignedRole = await userService.GetAvailableRoles();
            response.EmployeeId = employee.Id;
            response.EmployeeName = employee.FullName;
            response.EmployeeNo = employee.Id;
            response.Designation = assignedRole.Count > 0 ? assignedRole.FirstOrDefault().Name : string.Empty;
            response.JoinDate = employee.HireDate.HasValue ? employee.HireDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            response.PayDate = now.ToString("yyyy-MM-dd");
            response.PayPeriod = startOfMonth.ToString("yyyy-MM-dd") + "-" + endOfMonth.ToString("yyyy-MM-dd");
            response.PaymentMethod = "Bank Transfer";
            response.DaysWorked = @"22//22";
            response.LeaveTaken = "0";


            var primaryBankAccount = context.EmployeeBankAccounts
                .FirstOrDefault(x => x.EmployeeId == userId && x.IsPrimaryAccount);

            if (primaryBankAccount is not null)
            {
                response.BankName = primaryBankAccount.BankName;
                response.AccountNumber = primaryBankAccount.AccountNumber;
                response.Branch = primaryBankAccount.BranchName;
            }

            if (employee.EmployeeSalaries.Count > 0)
            {
                var employeeSalary = employee.EmployeeSalaries.FirstOrDefault();

                response.Earnings.Add(new PaymentDescriptionDTO() { Amount = employeeSalary is not null ? employeeSalary.BaseSalary : 0 });

                foreach (var addon in employeeSalary.EmployeeSalaryAddons)
                {
                    switch (addon.SalaryAddon.AddonType)
                    {
                        case SalaryAddonType.Allowance:
                            {
                                response.Earnings.Add(new PaymentDescriptionDTO()
                                {
                                    Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeSalary.BaseSalary * addon.AdjustedValue) / 100,
                                    Description = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.SalaryAddon.Name : $"{addon.SalaryAddon.Name} ({addon.AdjustedValue}%)"
                                });
                            }
                            break;
                        case SalaryAddonType.Deduction:
                        case SalaryAddonType.SocialSecuritySchemesEmployeeShare:
                            {
                                response.Deductions.Add(new PaymentDescriptionDTO()
                                {
                                    Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeSalary.BaseSalary * addon.AdjustedValue) / 100,
                                    Description = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.SalaryAddon.Name : $"{addon.SalaryAddon.Name} ({addon.AdjustedValue}%)"
                                });
                            }
                            break;
                        case SalaryAddonType.SocialSecuritySchemesCompanyShare:
                            {
                                response.EmployerContributions.Add(new PaymentDescriptionDTO()
                                {
                                    Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeSalary.BaseSalary * addon.AdjustedValue) / 100,
                                    Description = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.SalaryAddon.Name : $"{addon.SalaryAddon.Name} ({addon.AdjustedValue}%)"
                                });
                            }
                            break;
                    }
                }
            }

            response.NetSalary = response.Earnings.Sum(x => x.Amount) - response.Deductions.Sum(x => x.Amount);


            return response;
        }

        public void ConvertWordToPdf(string wordPath, string pdfPath)
        {
            using (FileStream docStream = new FileStream(wordPath, FileMode.Open, FileAccess.Read))
            {
                //Loads file stream into Word document
                using (WordDocument wordDocument = new WordDocument(docStream, FormatType.Docx))
                {
                    //Instantiation of DocIORenderer for Word to PDF conversion
                    using (DocIORenderer render = new DocIORenderer())
                    {
                        //Converts Word document into PDF document
                        Syncfusion.Pdf.PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);

                        using (FileStream outputStream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
                        {
                            pdfDocument.Save(outputStream);
                        }

                        //Saves the PDF document to MemoryStream.
                        //MemoryStream stream = new MemoryStream();
                        //pdfDocument.Save(stream);
                        //stream.Position = 0;

                        //Download PDF document in the browser.
                        //return File(stream, "application/pdf", pdfPath);
                    }
                }
            }
        }

        public async Task<EmployeeSalaryDTO> GetEmployeeSalaryByIdAsync(string userId)
        {
            var response = new EmployeeSalaryDTO();

            var employeeSalary = await context.EmployeeSalaries.FirstOrDefaultAsync(x => x.UserId == userId);
            if (employeeSalary == null)
            {

                response.UserId = userId;
                response.EffectiveFrom = DateTime.Now;

                var commonAddons = await context
                    .SalaryAddons
                    .Where(x => x.IsActive && x.ApplyForAllEmployees)
                    .ToListAsync(CancellationToken.None);

                foreach (var addon in commonAddons)
                {
                    var propostionType = addon.ProportionType == Domain.Enum.ProportionType.FixedAmount ? "$" : "%";
                    response.EmployeeSalaryAddons.Add(new EmployeeSalaryAddonDTO()
                    {
                        SalaryAddon = $"{addon.Name} ({propostionType})",
                        SalaryAddonId = addon.Id,
                        OriginalValue = addon.DefaultValue,
                        AdjustedValue = addon.DefaultValue,
                        ApplyForAllEmployees = addon.ApplyForAllEmployees,
                        EffectiveFrom = DateTime.Now
                    });
                }
            }
            else
            {
                response.Id = employeeSalary.Id;
                response.UserId = employeeSalary.UserId;
                response.BasicSalary = employeeSalary.BaseSalary;
                response.EffectiveFrom = employeeSalary.EffectiveFrom;
                response.CreatedOn = employeeSalary.CreatedDate.ToString("yyyy-MM-dd");
                response.UpdatedOn = employeeSalary.UpdateDate.Value.ToString("yyyy-MM-dd");
                response.Status = employeeSalary.Status;
                response.StatusString = EnumHelper.GetEnumDescription(employeeSalary.Status);

                foreach (var addon in employeeSalary.EmployeeSalaryAddons)
                {
                    var salaryAddon = await context.SalaryAddons.FirstOrDefaultAsync(x => x.Id == addon.SalaryAddonId);
                    var propostionType = salaryAddon.ProportionType == Domain.Enum.ProportionType.FixedAmount ? "$" : "%";
                    response.EmployeeSalaryAddons.Add(new EmployeeSalaryAddonDTO()
                    {
                        Id = addon.Id,
                        SalaryAddon = $"{salaryAddon.Name} ({propostionType})",
                        SalaryAddonId = addon.SalaryAddonId,
                        OriginalValue = addon.OriginalValue,
                        AdjustedValue = addon.AdjustedValue,
                        EffectiveFrom = addon.EffectiveFrom,
                        ApplyForAllEmployees = salaryAddon.ApplyForAllEmployees,
                        CreatedOn = addon.CreatedDate.ToString("yyyy-MM-dd"),
                        UpdatedOn = addon.UpdateDate.Value.ToString("yyyy-MM-dd")
                    });
                }
            }

            return response;
        }

        public Task<EmployeeSalarySlipDTO> GetEmployeeSalarySlip(string userId, int year, int month)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EmployeeSalaryAddonDTO>> GetUnAssignedSalaryAddonsAsync(string userId)
        {
            var assignedAddonIds = await context.EmployeeSalaries
                .Where(x => x.UserId == userId)
                .SelectMany(x => x.EmployeeSalaryAddons).Where(x => x.IsActive).Select(y => y.SalaryAddonId)
                .ToListAsync(CancellationToken.None);

            var unAssignedAddons = await context.SalaryAddons
                .Where(x => x.IsActive && !x.ApplyForAllEmployees && !assignedAddonIds.Contains(x.Id))
                .Select(x => new EmployeeSalaryAddonDTO()
                {
                    Id = 0,
                    SalaryAddon = x.Name + " (" + (x.ProportionType == Domain.Enum.ProportionType.FixedAmount ? "$" : "%") + ")",
                    SalaryAddonId = x.Id,
                    OriginalValue = x.DefaultValue,
                    AdjustedValue = x.DefaultValue,
                    EffectiveFrom = DateTime.Now,
                    ApplyForAllEmployees = x.ApplyForAllEmployees
                })
                .ToListAsync(CancellationToken.None);

            return unAssignedAddons;
        }

        public Task<GeneralResponseDTO> SaveUserSalaryAddonAsync(EmployeeSalaryAddonDTO salaryAddon)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponseDTO> SaveUserSalaryAsync(EmployeeSalaryDTO salary)
        {
            try
            {
                var employeeSalary = await context.EmployeeSalaries.FindAsync(salary.Id);
                if (employeeSalary == null)
                {
                    employeeSalary = new Domain.Entity.EmployeeSalary()
                    {
                        UserId = salary.UserId,
                        BaseSalary = salary.BasicSalary,
                        EffectiveFrom = salary.EffectiveFrom,
                        Status = Domain.Enum.EmployeeSalaryStatus.SubmittedForApproval,
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedByUserId = currentUserService.UserId,
                        UpdateDate = DateTime.Now,
                        IsActive = true
                    };

                    foreach (var addon in salary.EmployeeSalaryAddons)
                    {
                        employeeSalary.EmployeeSalaryAddons.Add(new Domain.Entity.EmployeeSalaryAddon()
                        {
                            SalaryAddonId = addon.SalaryAddonId,
                            OriginalValue = addon.OriginalValue,
                            AdjustedValue = addon.AdjustedValue,
                            EffectiveFrom = addon.EffectiveFrom,
                            CreatedByUserId = currentUserService.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedByUserId = currentUserService.UserId,
                            UpdateDate = DateTime.Now,
                            IsActive = true
                        });
                    }

                    context.EmployeeSalaries.Add(employeeSalary);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO() { Flag = true, Message = "User salary successfully inserted." };
                }
                else
                {
                    await AddEmployeeSalaryHistoryRecord(employeeSalary);

                    employeeSalary.BaseSalary = salary.BasicSalary;
                    employeeSalary.EffectiveFrom = salary.EffectiveFrom;
                    employeeSalary.UpdatedByUserId = currentUserService.UserId;
                    employeeSalary.UpdateDate = DateTime.Now;
                    employeeSalary.Status = Domain.Enum.EmployeeSalaryStatus.SubmittedForApproval;

                    var currentlySavedAddons = employeeSalary.EmployeeSalaryAddons.ToList();

                    var newlyAddedAddons = salary.EmployeeSalaryAddons.Where(x => x.Id <= 0);

                    var deletedAddons = (
                    from d in employeeSalary.EmployeeSalaryAddons
                    where !salary.EmployeeSalaryAddons.Any(x => x.Id == d.Id)
                    select d).ToList();

                    var updatedAddons = (
                        from u in salary.EmployeeSalaryAddons
                        where employeeSalary.EmployeeSalaryAddons.Any(x => x.Id == u.Id)
                        select u).ToList();

                    foreach (var addon in newlyAddedAddons)
                    {
                        //employeeSalary.EmployeeSalaryAddons.Add(new Domain.Entity.EmployeeSalaryAddon()
                        //{
                        //    SalaryAddonId = addon.SalaryAddonId,
                        //    OriginalValue = addon.OriginalValue,
                        //    AdjustedValue = addon.AdjustedValue,
                        //    EffectiveFrom = addon.EffectiveFrom,
                        //    CreatedByUserId = currentUserService.UserId,
                        //    CreatedDate = DateTime.Now,
                        //    UpdatedByUserId = currentUserService.UserId,
                        //    UpdateDate = DateTime.Now,
                        //    IsActive = true
                        //});

                        context.EmployeeSalaryAddons.Add(new Domain.Entity.EmployeeSalaryAddon()
                        {
                            EmployeeSalaryId = employeeSalary.Id,
                            SalaryAddonId = addon.SalaryAddonId,
                            OriginalValue = addon.OriginalValue,
                            AdjustedValue = addon.AdjustedValue,
                            EffectiveFrom = addon.EffectiveFrom,
                            CreatedByUserId = currentUserService.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedByUserId = currentUserService.UserId,
                            UpdateDate = DateTime.Now,
                            IsActive = true
                        });
                    }

                    await context.SaveChangesAsync(CancellationToken.None);



                    foreach (var addon in updatedAddons)
                    {
                        var existingAddon = employeeSalary.EmployeeSalaryAddons.FirstOrDefault(x => x.Id == addon.Id);
                        if (existingAddon != null)
                        {
                            existingAddon.SalaryAddonId = addon.SalaryAddonId;
                            existingAddon.OriginalValue = addon.OriginalValue;
                            existingAddon.AdjustedValue = addon.AdjustedValue;
                            existingAddon.EffectiveFrom = addon.EffectiveFrom;
                            existingAddon.UpdatedByUserId = currentUserService.UserId;
                            existingAddon.UpdateDate = DateTime.Now;

                            context.EmployeeSalaryAddons.Update(existingAddon);
                        }
                    }

                    await context.SaveChangesAsync(CancellationToken.None);



                    foreach (var deletedAddon in deletedAddons)
                    {
                        context.EmployeeSalaryAddons.Remove(deletedAddon);
                    }

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO() { Flag = true, Message = "User salary successfully updated." };
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }

        }

        private async Task AddEmployeeSalaryHistoryRecord(EmployeeSalary employeeSalary)
        {
            var employeeSalaryHistory = new EmployeeSalaryHistory()
            {
                EmployeeSalaryId = employeeSalary.Id,
                UserId = employeeSalary.UserId,
                BaseSalary = employeeSalary.BaseSalary,
                EffectiveFrom = employeeSalary.EffectiveFrom,
                Status = employeeSalary.Status,
                CreatedByUserId = employeeSalary.CreatedByUserId,
                CreatedDate = employeeSalary.CreatedDate,
                UpdatedByUserId = employeeSalary.UpdatedByUserId,
                UpdateDate = employeeSalary.UpdateDate,
                IsActive = employeeSalary.IsActive
            };

            foreach (var addon in employeeSalary.EmployeeSalaryAddons)
            {
                employeeSalaryHistory.EmployeeSalaryAddonHistories.Add(new EmployeeSalaryAddonHistory()
                {
                    EmployeeSalaryId = addon.EmployeeSalaryId,
                    SalaryAddonId = addon.SalaryAddonId,
                    OriginalValue = addon.OriginalValue,
                    AdjustedValue = addon.AdjustedValue,
                    EffectiveFrom = addon.EffectiveFrom,
                    CreatedByUserId = addon.CreatedByUserId,
                    CreatedDate = addon.CreatedDate,
                    UpdatedByUserId = addon.UpdatedByUserId,
                    UpdateDate = addon.UpdateDate,
                    IsActive = addon.IsActive
                });
            }

            context.EmployeeSalaryHistories.Add(employeeSalaryHistory);

            await context.SaveChangesAsync(CancellationToken.None);

        }

        private void ReplaceText(MainDocumentPart mainPart, string placeholder, string replacement)
        {
            string docText = null;
            using (StreamReader sr = new StreamReader(mainPart.GetStream()))
            {
                docText = sr.ReadToEnd();
            }

            docText = docText.Replace(placeholder, replacement);

            using (StreamWriter sw = new StreamWriter(mainPart.GetStream(FileMode.Create)))
            {
                sw.Write(docText);
            }
        }

        // Helper method to fill a value in a specific cell
        private void FillCellValue(TableRow row, int cellIndex, string value)
        {
            TableCell cell = row.Elements<TableCell>().ElementAtOrDefault(cellIndex);
            if (cell != null)
            {
                // Get the first paragraph in the cell or create one if it doesn't exist
                Paragraph paragraph = cell.Elements<Paragraph>().FirstOrDefault();
                if (paragraph == null)
                {
                    paragraph = new Paragraph();
                    cell.Append(paragraph);
                }

                // Clear existing content
                paragraph.RemoveAllChildren<Run>();

                // Add new text
                Run run = new Run(new Text(value));
                paragraph.Append(run);
            }
        }

    }
}
