using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Extensions.Helpers.OpenXML;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.Reflection;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using JustificationValues = DocumentFormat.OpenXml.Wordprocessing.JustificationValues;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace StaffApp.Infrastructure.Services
{
    public class UserSalaryService(
        IStaffAppDbContext context,
        IUserService userService,
        ICurrentUserService currentUserService,
        IFileDownloadService fileDownloadService,
        ILogger<IUserSalaryService> userSalaryServiceLogger) : IUserSalaryService
    {
        #region Public Methods
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

        public async Task<DocumentDTO> GenerateEstimateSalarySlipAsync(EmployeeSalarySlipDTO salarySlip)
        {
            string filePath = string.Empty;
            var documentDto = new DocumentDTO();

            try
            {
                var outPutDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

                var templatePath = System.IO.Path.Combine(outPutDirectory, @"WordTemplates\SalarySlipTemplate.docx");

                var salarySlipPath = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SalarySlipFolderPath);
                // Create a temporary file for the modified document
                string fileName = $"SalarySlip_{Guid.NewGuid().ToString()}.docx";
                documentDto.FileName = System.IO.Path.ChangeExtension(fileName, ".pdf");
                string tempDocxPath = System.IO.Path.Combine(salarySlipPath.Value, fileName);
                string tempPdfPath = System.IO.Path.Combine(salarySlipPath.Value, System.IO.Path.ChangeExtension(fileName, ".pdf"));

                File.Copy(templatePath, tempDocxPath, true);

                // Modify the document using OpenXML
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(tempDocxPath, true))
                {
                    // Get the main document part
                    MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                    ReplaceText(mainPart, "CompanyName", salarySlip.CompanyName);
                    ReplaceText(mainPart, "CompanyAddress", salarySlip.CompanyAddress);
                    ReplaceText(mainPart, "Telephone", salarySlip.CompanyPhone);
                    ReplaceText(mainPart, "CompanyEmail", salarySlip.CompanyEmail);
                    ReplaceText(mainPart, "YearAndMonth", $"{salarySlip.SalarySlipYear}-{salarySlip.SalarySlipMonth}");
                    ReplaceText(mainPart, "SLIPNumber", salarySlip.SalarySlipNumber);

                    ReplaceText(mainPart, "EmployeeNumber", salarySlip.EmployeeId);
                    ReplaceText(mainPart, "EmployeeName", salarySlip.EmployeeName);
                    ReplaceText(mainPart, "EmployeeDesignation", salarySlip.Designation);
                    ReplaceText(mainPart, "EmployeeDepartment", string.Empty);
                    ReplaceText(mainPart, "JoinDate", salarySlip.JoinDate);

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
                        //Employee Earnings
                        Table earningTable = tables[5];

                        for (int i = 0; i < salarySlip.Earnings.Count(); i++)
                        {
                            if (i == 0)
                            {
                                DocumentFormat.OpenXml.Wordprocessing.TableRow earningRow = earningTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);
                                // Fill values in specific cells
                                FillCellValue(earningRow, 0, salarySlip.Earnings[i].Description);      // First column
                                FillCellValue(earningRow, 1, salarySlip.Earnings[i].Amount.ToString("C"));   // Third column
                            }
                            else
                            {
                                TableRow newRow = new TableRow();

                                // Add cells to the new row
                                string[] cellValues = new string[] { salarySlip.Earnings[i].Description, salarySlip.Earnings[i].Amount.ToString("C") };
                                int index = 0;
                                foreach (string value in cellValues)
                                {
                                    var runProperties = new RunProperties(new Bold() { Val = false });
                                    TableCell cell = new TableCell(new Paragraph(new Run(runProperties, new Text(value))));
                                    OpenXMLTableHelper.SetCellFontSize(cell, 10);
                                    //OpenXMLTableHelper.RemoveBoldFromTableCell(cell);
                                    if (index == 1)
                                    {
                                        OpenXMLTableHelper.SetHorizontalTextAlignment(cell, JustificationValues.Right);
                                    }
                                    newRow.Append(cell);
                                    index++;
                                }

                                earningTable.Append(newRow);
                            }
                        }

                        //Deduction
                        Table deductionTable = tables[6];

                        for (int i = 0; i < salarySlip.Deductions.Count; i++)
                        {
                            if (i == 0)
                            {
                                DocumentFormat.OpenXml.Wordprocessing.TableRow deductionRow = deductionTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);
                                // Fill values in specific cells
                                FillCellValue(deductionRow, 0, salarySlip.Deductions[i].Description);      // First column
                                FillCellValue(deductionRow, 1, salarySlip.Deductions[i].Amount.ToString("C"));   // Third column
                            }
                            else
                            {
                                TableRow newRow = new TableRow();
                                // Add cells to the new row
                                string[] cellValues = new string[] { salarySlip.Deductions[i].Description, salarySlip.Deductions[i].Amount.ToString("C") };
                                int index = 0;
                                foreach (string value in cellValues)
                                {
                                    var runProperties = new RunProperties(new Bold() { Val = false });
                                    TableCell cell = new TableCell(new Paragraph(new Run(runProperties, new Text(value))));
                                    OpenXMLTableHelper.SetCellFontSize(cell, 10);
                                    //OpenXMLTableHelper.RemoveBoldFromTableCell(cell);
                                    if (index == 1)
                                    {
                                        OpenXMLTableHelper.SetHorizontalTextAlignment(cell, JustificationValues.Right);
                                    }
                                    newRow.Append(cell);
                                    index++;
                                }
                                deductionTable.Append(newRow);
                            }
                        }


                        //Employer Contribution
                        Table contributionTable = tables[8];
                        for (int i = 0; i < salarySlip.EmployerContributions.Count; i++)
                        {
                            if (i == 0)
                            {
                                DocumentFormat.OpenXml.Wordprocessing.TableRow contributionRow = contributionTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);
                                // Fill values in specific cells
                                FillCellValue(contributionRow, 0, salarySlip.EmployerContributions[i].Description);      // First column
                                FillCellValue(contributionRow, 1, salarySlip.EmployerContributions[i].Amount.ToString("C"));   // Third column
                            }
                            else
                            {
                                TableRow newRow = new TableRow();
                                // Add cells to the new row
                                string[] cellValues = new string[] { salarySlip.EmployerContributions[i].Description, salarySlip.EmployerContributions[i].Amount.ToString("C") };
                                int index = 0;
                                foreach (string value in cellValues)
                                {
                                    var runProperties = new RunProperties(new Bold() { Val = false });
                                    TableCell cell = new TableCell(new Paragraph(new Run(runProperties, new Text(value))));
                                    OpenXMLTableHelper.SetCellFontSize(cell, 10);
                                    //OpenXMLTableHelper.RemoveBoldFromTableCell(cell);
                                    if (index == 1)
                                    {
                                        OpenXMLTableHelper.SetHorizontalTextAlignment(cell, JustificationValues.Right);
                                    }
                                    newRow.Append(cell);
                                    index++;
                                }
                                contributionTable.Append(newRow);
                            }
                        }

                        //Table fourthTable = tables[10];

                        //DocumentFormat.OpenXml.Wordprocessing.TableRow row3 = fourthTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);

                        //if (row3 != null)
                        //{
                        //    // Fill values in specific cells
                        //    FillCellValue(row3, 0, "John Doe444");      // First column
                        //    FillCellValue(row3, 1, "Employee444");   // Third column
                        //}
                    }

                    // Save the changes
                    mainPart.Document.Save();
                }
                // Convert the modified document to PDF
                ConvertWordToPdf(tempDocxPath, tempPdfPath);

                documentDto.FileArray = await fileDownloadService.GetFileAsync(tempPdfPath);

            }
            catch (Exception ex)
            {
                userSalaryServiceLogger.LogError(ex.ToString());
            }

            return documentDto;
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

        public async Task<EmployeeSalarySlipDTO> GetEmployeeEstimateSalarySlipAsync(string userId)
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

            response.SalarySlipMonth = "XX";
            response.SalarySlipYear = "20XX";
            response.SalarySlipNumber = "####";

            if (employee == null)
                return response;



            DateTime now = DateTime.Now;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var assignedRole = await userService.GetAvailableRoles();
            response.EmployeeId = employee.EmployeeNumber.ToString();
            response.EmployeeName = employee.FullName;
            response.EmployeeNo = employee.Id;
            response.Designation = assignedRole.Count > 0 ? assignedRole.FirstOrDefault().Name : string.Empty;
            response.JoinDate = employee.HireDate.HasValue ? employee.HireDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            response.PayDate = now.ToString("yyyy-MM-dd");
            response.PayPeriod = startOfMonth.ToString("yyyy-MM-dd") + "-" + endOfMonth.ToString("yyyy-MM-dd");
            response.PaymentMethod = "Bank Transfer";
            response.DaysWorked = @"22/22";
            response.LeaveTaken = "N/A";


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

                response.Earnings.Add(new PaymentDescriptionDTO() { Amount = employeeSalary is not null ? employeeSalary.BaseSalary : 0, Description = "Basic Salary" });
                var taxSalaryAddons = new List<EmployeeSalaryAddon>();

                foreach (var addon in employeeSalary.EmployeeSalaryAddons)
                {
                    var paymentDescription = new PaymentDescriptionDTO()
                    {
                        SalaryAddonType = addon.SalaryAddon.AddonType,
                        Amount = 0,
                        Description = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.SalaryAddon.Name : $"{addon.SalaryAddon.Name} ({addon.AdjustedValue}%)"
                    };
                    switch (addon.SalaryAddon.AddonType)
                    {
                        case SalaryAddonType.Allowance:
                            {
                                paymentDescription.Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeSalary.BaseSalary * addon.AdjustedValue) / 100;
                                response.Earnings.Add(paymentDescription);
                            }
                            break;
                        case SalaryAddonType.Tax:
                        case SalaryAddonType.Deduction:
                        case SalaryAddonType.SocialSecuritySchemesEmployeeShare:
                            {
                                if (addon.SalaryAddon.AddonType != SalaryAddonType.Tax)
                                {
                                    paymentDescription.Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeSalary.BaseSalary * addon.AdjustedValue) / 100;
                                }
                                else
                                {
                                    taxSalaryAddons.Add(addon);
                                }
                                response.Deductions.Add(paymentDescription);
                            }
                            break;
                        case SalaryAddonType.SocialSecuritySchemesCompanyShare:
                            {
                                paymentDescription.Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeSalary.BaseSalary * addon.AdjustedValue) / 100;
                                response.EmployerContributions.Add(paymentDescription);
                            }
                            break;
                    }
                }

                foreach (var taxAddon in taxSalaryAddons)
                {
                    var deductionRecord = response.Deductions.FirstOrDefault(x => x.SalaryAddonType == taxAddon.SalaryAddon.AddonType);

                    switch (taxAddon.SalaryAddon.ProportionType)
                    {
                        case ProportionType.FixedAmount:
                            {
                                deductionRecord.Amount = taxAddon.AdjustedValue;
                            }
                            break;
                        case ProportionType.Percentage:
                            {
                                deductionRecord.Amount = (response.Earnings.Sum(x => x.Amount) * taxAddon.AdjustedValue) / 100.00m;
                            }
                            break;
                        case ProportionType.MultipleRange:
                            {
                                deductionRecord.Amount = CalculateMonthlyTaxForMultipleRange(response.Earnings.Sum(x => x.Amount), taxAddon.SalaryAddon.TaxLogics.ToList());
                            }
                            break;
                    }
                }

                response.NetSalary = response.Earnings.Sum(x => x.Amount) - response.Deductions.Sum(x => x.Amount);
            }

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

        public async Task<EmployeeSalarySlipDTO> GetEmployeeSalarySlipAsync(int employeeMonthlySalaryId)
        {
            var response = new EmployeeSalarySlipDTO();

            var employeeMonthlySalary = await context.EmployeeMonthlySalaries.FindAsync(employeeMonthlySalaryId);


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

            response.SalarySlipMonth = employeeMonthlySalary.MonthlySalary.Month.ToString();
            response.SalarySlipYear = employeeMonthlySalary.MonthlySalary.CompanyYear.Year.ToString();
            response.SalarySlipNumber = "SLIP-" + employeeMonthlySalary.Id.ToString("D5"); ;

            if (employeeMonthlySalary == null)
                return response;

            DateTime now = DateTime.Now;
            DateTime startOfMonth = new DateTime(now.Year, (int)employeeMonthlySalary.MonthlySalary.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var assignedRole = await userService.GetAvailableRoles();
            response.EmployeeId = employeeMonthlySalary.EmployeeSalary.User.EmployeeNumber.ToString();
            response.EmployeeName = employeeMonthlySalary.EmployeeSalary.User.FullName;
            response.EmployeeNo = employeeMonthlySalary.EmployeeSalary.User.EmployeeNumber.ToString();
            response.Designation = assignedRole.Count > 0 ? assignedRole.FirstOrDefault().Name : string.Empty;
            response.JoinDate = employeeMonthlySalary.EmployeeSalary.User.HireDate.HasValue ? employeeMonthlySalary.EmployeeSalary.User.HireDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            response.PayDate = now.ToString("yyyy-MM-dd");
            response.PayPeriod = startOfMonth.ToString("yyyy-MM-dd") + "-" + endOfMonth.ToString("yyyy-MM-dd");
            response.PaymentMethod = "Bank Transfer";
            response.DaysWorked = @"22/22";
            response.LeaveTaken = "N/A";


            var primaryBankAccount = context.EmployeeBankAccounts
                .FirstOrDefault(x => x.EmployeeId == employeeMonthlySalary.EmployeeSalary.User.Id && x.IsPrimaryAccount);

            if (primaryBankAccount is not null)
            {
                response.BankName = primaryBankAccount.BankName;
                response.AccountNumber = primaryBankAccount.AccountNumber;
                response.Branch = primaryBankAccount.BranchName;
            }


            response.Earnings.Add(new PaymentDescriptionDTO() { Amount = employeeMonthlySalary is not null ? employeeMonthlySalary.BasicSalary : 0, Description = "Basic Salary" });
            var employeeTaxSalaryAddons = new List<EmployeeMonthlySalaryAddon>();

            foreach (var addon in employeeMonthlySalary.EmployeeMonthlySalaryAddons)
            {
                var paymentDescription = new PaymentDescriptionDTO()
                {
                    SalaryAddonType = addon.SalaryAddon.AddonType,
                    Amount = 0,
                    Description = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.SalaryAddon.Name : $"{addon.SalaryAddon.Name} ({addon.AdjustedValue}%)"
                };
                switch (addon.SalaryAddon.AddonType)
                {
                    case SalaryAddonType.Allowance:
                        {
                            paymentDescription.Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeMonthlySalary.BasicSalary * addon.AdjustedValue) / 100.00m;
                            response.Earnings.Add(paymentDescription);
                        }
                        break;
                    case SalaryAddonType.Tax:
                    case SalaryAddonType.Deduction:
                    case SalaryAddonType.SocialSecuritySchemesEmployeeShare:
                        {
                            if (addon.SalaryAddon.AddonType != SalaryAddonType.Tax)
                            {
                                paymentDescription.Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeMonthlySalary.BasicSalary * addon.AdjustedValue) / 100.00m;
                            }
                            else
                            {
                                employeeTaxSalaryAddons.Add(addon);
                            }
                            response.Deductions.Add(paymentDescription);
                        }
                        break;
                    case SalaryAddonType.SocialSecuritySchemesCompanyShare:
                        {
                            paymentDescription.Amount = addon.SalaryAddon.ProportionType == ProportionType.FixedAmount ? addon.AdjustedValue : (employeeMonthlySalary.BasicSalary * addon.AdjustedValue) / 100.00m;
                            response.EmployerContributions.Add(paymentDescription);
                        }
                        break;
                }
            }

            foreach (var taxAddon in employeeTaxSalaryAddons)
            {
                var deductionRecord = response.Deductions.FirstOrDefault(x => x.SalaryAddonType == taxAddon.SalaryAddon.AddonType);

                switch (taxAddon.SalaryAddon.ProportionType)
                {
                    case ProportionType.FixedAmount:
                        {
                            deductionRecord.Amount = taxAddon.AdjustedValue;
                        }
                        break;
                    case ProportionType.Percentage:
                        {
                            deductionRecord.Amount = (response.Earnings.Sum(x => x.Amount) * taxAddon.AdjustedValue) / 100.00m;
                        }
                        break;
                    case ProportionType.MultipleRange:
                        {
                            deductionRecord.Amount = CalculateMonthlyTaxForMultipleRange(response.Earnings.Sum(x => x.Amount), taxAddon.SalaryAddon.TaxLogics.ToList());
                        }
                        break;
                }
            }

            response.NetSalary = response.Earnings.Sum(x => x.Amount) - response.Deductions.Sum(x => x.Amount);


            return response;
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

        public async Task<List<EmployeeMonthlySalaryAddonDTO>> GetUnAssignedMonthlySalaryAddonsAsync(int employeeMonthlySalaryId)
        {
            var assignedAddonIds = (await context.EmployeeMonthlySalaries
                .FirstOrDefaultAsync(x => x.Id == employeeMonthlySalaryId, CancellationToken.None))
                .EmployeeMonthlySalaryAddons
                .Where(x => x.IsActive).Select(y => y.SalaryAddonId);

            var unAssignedAddons = await context.SalaryAddons
                .Where(x => x.IsActive && !x.ApplyForAllEmployees && !assignedAddonIds.Contains(x.Id))
                .Select(x => new EmployeeMonthlySalaryAddonDTO()
                {
                    Id = 0,
                    SalaryAddon = x.Name + " (" + (x.ProportionType == Domain.Enum.ProportionType.FixedAmount ? "$" : "%") + ")",
                    SalaryAddonId = x.Id,
                    OriginalValue = x.DefaultValue,
                    AdjustedValue = x.DefaultValue,
                    Amount = 0,
                    EmployeeMonthlySalaryId = employeeMonthlySalaryId,
                    ApplyForAllEmployees = x.ApplyForAllEmployees,
                    ProportionType = x.ProportionType
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

        public async Task<GeneralResponseDTO> GenerateEmployeesMonthSalary(int year, int month)
        {
            try
            {
                var employeeSalaries = await context
                    .EmployeeSalaries.Where(x => x.User.IsActive && x.IsActive)
                    .ToListAsync(CancellationToken.None);

                var monthlySalary = await context.MonthlySalaries.FirstOrDefaultAsync(x => x.CompanyYearId == year && x.Month == (Month)month);
                if (monthlySalary == null)
                {
                    monthlySalary = new MonthlySalary()
                    {
                        IsActive = true,
                        CompanyYearId = year,
                        Month = (Month)month,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = currentUserService.UserId,
                        UpdateDate = DateTime.Now,
                        UpdatedByUserId = currentUserService.UserId,
                        Status = MonthlySalaryStatus.Generated
                    };
                }

                foreach (var item in employeeSalaries)
                {
                    var employeeMonthlySalary = await context.EmployeeMonthlySalaries
                        .FirstOrDefaultAsync(x => x.EmployeeSalaryId == item.Id && x.MonthlySalary.CompanyYearId == year && x.MonthlySalary.Month == (Month)month);

                    if (employeeMonthlySalary is null)
                    {
                        var monthlyPaymentAddons = new List<EmployeeMonthlySalaryAddon>();

                        var totalAllowances = 0.00m;
                        var totalDeductions = 0.00m;
                        var totalEmployerContributions = 0.00m;
                        var employeeTaxSalaryAddons = new List<EmployeeSalaryAddon>();
                        foreach (var addon in item.EmployeeSalaryAddons)
                        {
                            var amount = addon.AdjustedValue;
                            if (addon.SalaryAddon.AddonType == SalaryAddonType.Tax)
                            {

                                //var totalAllowances = item.EmployeeSalaryAddons.Where(x => x.SalaryAddon.AddonType == SalaryAddonType.Allowance).Sum(x => x.AdjustedValue);
                                employeeTaxSalaryAddons.Add(addon);
                                amount = 0;
                            }
                            else if (addon.SalaryAddon.ProportionType == ProportionType.Percentage)
                            {
                                amount = (item.BaseSalary * addon.AdjustedValue) / 100.00m;
                            }

                            if (addon.SalaryAddon.AddonType == SalaryAddonType.Allowance)
                            {
                                totalAllowances += amount;
                            }
                            else if (addon.SalaryAddon.AddonType == SalaryAddonType.Deduction || addon.SalaryAddon.AddonType == SalaryAddonType.Tax ||
                                addon.SalaryAddon.AddonType == SalaryAddonType.SocialSecuritySchemesEmployeeShare)
                            {
                                totalDeductions += amount;
                            }
                            else if (addon.SalaryAddon.AddonType == SalaryAddonType.SocialSecuritySchemesCompanyShare)
                            {
                                totalEmployerContributions += amount;
                            }


                            var monthlyAddon = new EmployeeMonthlySalaryAddon()
                            {
                                SalaryAddonId = addon.SalaryAddonId,
                                AdjustedValue = addon.AdjustedValue,
                                OriginalValue = addon.OriginalValue,
                                Amount = amount,
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = currentUserService.UserId,
                                UpdateDate = DateTime.Now,
                                UpdatedByUserId = currentUserService.UserId,
                                IsActive = true
                            };

                            monthlyPaymentAddons.Add(monthlyAddon);
                        }

                        foreach (var taxAddon in employeeTaxSalaryAddons)
                        {
                            var deductionPaymentAddon = monthlyPaymentAddons.FirstOrDefault(x => x.SalaryAddonId == taxAddon.SalaryAddonId);
                            if (deductionPaymentAddon != null)
                            {
                                var tax = 0.00m;

                                switch (taxAddon.SalaryAddon.ProportionType)
                                {
                                    case ProportionType.FixedAmount:
                                        {
                                            tax = taxAddon.AdjustedValue;
                                        }
                                        break;
                                    case ProportionType.Percentage:
                                        {
                                            tax = ((item.BaseSalary + totalAllowances) * taxAddon.AdjustedValue) / 100.00m;
                                        }
                                        break;
                                    case ProportionType.MultipleRange:
                                        {
                                            tax = CalculateMonthlyTaxForMultipleRange(item.BaseSalary + totalAllowances, taxAddon.SalaryAddon.TaxLogics.ToList());
                                        }
                                        break;
                                }

                                deductionPaymentAddon.Amount = tax;
                                totalDeductions += tax;
                            }
                        }

                        employeeMonthlySalary = new EmployeeMonthlySalary()
                        {
                            EmployeeSalaryId = item.Id,
                            BasicSalary = item.BaseSalary,
                            TotalEarning = totalAllowances + item.BaseSalary,
                            EmployerContribution = totalEmployerContributions,
                            TotalDeduction = totalDeductions,
                            NetSalary = (totalAllowances + item.BaseSalary) - totalDeductions,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = currentUserService.UserId,
                            UpdateDate = DateTime.Now,
                            UpdatedByUserId = currentUserService.UserId,
                            IsActive = true,
                            Status = EmployeeSalaryStatus.Generated
                        };

                        foreach (var addon in monthlyPaymentAddons)
                        {
                            employeeMonthlySalary.EmployeeMonthlySalaryAddons.Add(addon);
                        }

                        monthlySalary.EmployeeMonthlySalaries.Add(employeeMonthlySalary);
                    }
                }

                context.MonthlySalaries.Add(monthlySalary);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Employee Month Salary generated successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }

        }

        public async Task<GeneralResponseDTO> CheckEmployeesMonthlySalaryGeneratedAsync(int year, int month)
        {
            var IsEmployeeMonthlySalaryExsits = await context.EmployeeMonthlySalaries
                .AnyAsync(x => x.MonthlySalary.CompanyYearId == year && x.MonthlySalary.Month == (Month)month);

            return new GeneralResponseDTO()
            {
                Flag = IsEmployeeMonthlySalaryExsits,
                Message = IsEmployeeMonthlySalaryExsits ? "Employee Monthly Salary already generated." : "Employee Monthly Salary not generated."
            };
        }

        public async Task<PaginatedResultDTO<EmployeeMonthlySalarySummaryDTO>> GetMonthlyEmployeeSalaries(int year, int month, int pageNumber, int pageSize, string sortField = null, bool ascending = true)
        {
            var employeeSalaryQuery = context.EmployeeMonthlySalaries.Where(x => x.MonthlySalary.CompanyYearId == year && x.MonthlySalary.Month == (Month)month);

            int totalCount = await employeeSalaryQuery.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "fullName":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.EmployeeSalary.User.FullName)
                            : employeeSalaryQuery.OrderByDescending(u => u.EmployeeSalary.User.FullName);
                        break;
                    case "basicSalary":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.BasicSalary)
                            : employeeSalaryQuery.OrderByDescending(u => u.BasicSalary);
                        break;
                    case "totalDeduction":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.TotalDeduction)
                            : employeeSalaryQuery.OrderByDescending(u => u.TotalDeduction);
                        break;
                    case "employerContribution":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.EmployerContribution)
                            : employeeSalaryQuery.OrderByDescending(u => u.EmployerContribution);
                        break;
                    case "netSalary":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.NetSalary)
                            : employeeSalaryQuery.OrderByDescending(u => u.NetSalary);
                        break;
                    case "employeeNo":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.EmployeeSalary.User.Id)
                            : employeeSalaryQuery.OrderByDescending(u => u.EmployeeSalary.User.Id);
                        break;
                    default:
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.Id)
                            : employeeSalaryQuery.OrderByDescending(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sorting by full name
                employeeSalaryQuery = employeeSalaryQuery.OrderBy(u => u.EmployeeSalary.User.FullName);
            }

            var items = await employeeSalaryQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(salary => new EmployeeMonthlySalarySummaryDTO
            {
                Id = salary.Id,
                EmployeeName = salary.EmployeeSalary.User.FullName,
                EmployeeNumber = salary.EmployeeSalary.User.EmployeeNumber.ToString(),
                NetSalary = salary.NetSalary.ToString("C") + (salary.IsRevised ? " (Revised)" : string.Empty),
                BasicSalary = salary.BasicSalary.ToString("C"),
                TotalDeduction = salary.TotalDeduction.ToString("C"),
                TotalEarning = salary.TotalEarning.ToString("C"),
                EmployerContribution = salary.EmployerContribution.ToString("C"),
                IsRevised = salary.IsRevised
            }).ToListAsync();

            var newResult = new PaginatedResultDTO<EmployeeMonthlySalarySummaryDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<PaginatedResultDTO<EmployeeMonthlySalarySummaryDTO>> GetMyMonthlySalaryList(int pageNumber, int pageSize, string sortField = null, bool ascending = true)
        {
            var currentUserId = currentUserService.UserId;
            var employeeSalaryQuery = context.EmployeeMonthlySalaries
                .Where(x => x.EmployeeSalary.UserId == currentUserId);

            int totalCount = await employeeSalaryQuery.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "basicSalary":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.BasicSalary)
                            : employeeSalaryQuery.OrderByDescending(u => u.BasicSalary);
                        break;
                    case "totalDeduction":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.TotalDeduction)
                            : employeeSalaryQuery.OrderByDescending(u => u.TotalDeduction);
                        break;
                    case "employerContribution":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.EmployerContribution)
                            : employeeSalaryQuery.OrderByDescending(u => u.EmployerContribution);
                        break;
                    case "netSalary":
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.NetSalary)
                            : employeeSalaryQuery.OrderByDescending(u => u.NetSalary);
                        break;
                    default:
                        employeeSalaryQuery = ascending
                            ? employeeSalaryQuery.OrderBy(u => u.Id)
                            : employeeSalaryQuery.OrderByDescending(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sorting by full name
                employeeSalaryQuery = employeeSalaryQuery.OrderBy(u => u.EmployeeSalary.User.FullName);
            }

            var items = await employeeSalaryQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(salary => new EmployeeMonthlySalarySummaryDTO
            {
                Id = salary.Id,
                EmployeeName = salary.EmployeeSalary.User.FullName,
                EmployeeNumber = salary.EmployeeSalary.User.EmployeeNumber.ToString(),
                NetSalary = salary.NetSalary.ToString("C") + (salary.IsRevised ? " (Revised)" : string.Empty),
                BasicSalary = salary.BasicSalary.ToString("C"),
                TotalDeduction = salary.TotalDeduction.ToString("C"),
                TotalEarning = salary.TotalEarning.ToString("C"),
                EmployerContribution = salary.EmployerContribution.ToString("C"),
                IsRevised = salary.IsRevised,
                Year = salary.MonthlySalary.CompanyYearId.ToString(),
                Month = EnumHelper.GetEnumDescription(salary.MonthlySalary.Month)
            }).ToListAsync();

            var newResult = new PaginatedResultDTO<EmployeeMonthlySalarySummaryDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }
        public async Task<EmployeeMonthlySalaryStatusDTO> GetEmployeeMonthlySalaryStatus(int year, int month)
        {
            var employeeMonth = await context.MonthlySalaries.FirstOrDefaultAsync(x => x.CompanyYearId == year && x.Month == (Month)month);

            if (employeeMonth is null)
                return new EmployeeMonthlySalaryStatusDTO()
                {
                    Id = 0,
                    StatusName = EnumHelper.GetEnumDescription(MonthlySalaryStatus.NotGenerated),
                    Status = MonthlySalaryStatus.NotGenerated
                };

            var createdUser = await userService.GetUserByIdAsync(employeeMonth.CreatedByUserId);
            var updatedUser = await userService.GetUserByIdAsync(employeeMonth.UpdatedByUserId);

            return new EmployeeMonthlySalaryStatusDTO()
            {
                CreatedByUser = createdUser.FullName,
                CreatedDate = employeeMonth.CreatedDate.ToString("yyyy-MM-dd"),
                Id = employeeMonth.Id,
                StatusName = EnumHelper.GetEnumDescription(employeeMonth.Status),
                Status = employeeMonth.Status,
                UpdatedByUser = updatedUser.FullName,
                UpdatedDate = employeeMonth.UpdateDate.Value.ToString("yyyy-MM-dd")
            };
        }

        public async Task<GeneralResponseDTO> UpdateUserMonthlySalaryAsync(EmployeeMonthlySalaryDTO salary)
        {
            try
            {


                var alreadyAssignedAddons = salary.EmployeeSalaryAddons.Select(x => x.Id);

                var deletedEmployeeAddons = await context.EmployeeMonthlySalaryAddons.Where(x => x.EmployeeMonthlySalaryId == salary.Id && !alreadyAssignedAddons.Contains(x.Id)).ToListAsync(CancellationToken.None);

                foreach (var deletedItem in deletedEmployeeAddons)
                {
                    context.EmployeeMonthlySalaryAddons.Remove(deletedItem);
                }

                await context.SaveChangesAsync(CancellationToken.None);

                var employeeMonthlySalary = await context.EmployeeMonthlySalaries.FindAsync(salary.Id);

                var totalAllowances = 0.00m;
                var totalDeductions = 0.00m;
                var totalEmployerContributions = 0.00m;
                var employeeTaxSalaryAddons = new List<EmployeeMonthlySalaryAddon>();
                var taxSalaryAddons = new List<SalaryAddon>();

                foreach (var addon in salary.EmployeeSalaryAddons)
                {
                    var adjustedValue = addon.AdjustedValue;
                    var amount = addon.AdjustedValue;

                    if (addon.Id > 0)
                    {
                        var existingAddon = await context.EmployeeMonthlySalaryAddons.FindAsync(addon.Id);
                        if (existingAddon.SalaryAddon.AddonType == SalaryAddonType.Tax)
                        {
                            employeeTaxSalaryAddons.Add(existingAddon);
                            amount = 0.00m;
                        }
                        else
                        {

                            if (existingAddon.SalaryAddon.ProportionType == ProportionType.Percentage)
                            {
                                amount = (employeeMonthlySalary.BasicSalary * addon.AdjustedValue) / 100.00m;
                            }

                            if (existingAddon.SalaryAddon.AddonType == SalaryAddonType.Allowance)
                            {
                                totalAllowances += amount;
                            }
                            else if (existingAddon.SalaryAddon.AddonType == SalaryAddonType.Deduction ||
                                existingAddon.SalaryAddon.AddonType == SalaryAddonType.SocialSecuritySchemesEmployeeShare)
                            {
                                totalDeductions += amount;
                            }
                            else if (existingAddon.SalaryAddon.AddonType == SalaryAddonType.SocialSecuritySchemesCompanyShare)
                            {
                                totalEmployerContributions += amount;
                            }

                            existingAddon.AdjustedValue = adjustedValue;
                            existingAddon.Amount = amount;
                            existingAddon.OriginalValue = existingAddon.OriginalValue;
                            existingAddon.UpdateDate = DateTime.Now;
                            existingAddon.UpdatedByUserId = currentUserService.UserId;
                            context.EmployeeMonthlySalaryAddons.Update(existingAddon);
                        }
                    }
                    else
                    {
                        var salaryAddon = await context.SalaryAddons.FindAsync(addon.SalaryAddonId);

                        if (salaryAddon.AddonType == SalaryAddonType.Tax)
                        {
                            taxSalaryAddons.Add(salaryAddon);
                            amount = 0.00m;
                        }
                        else
                        {
                            if (salaryAddon.ProportionType == ProportionType.Percentage)
                            {
                                amount = (employeeMonthlySalary.BasicSalary * addon.AdjustedValue) / 100.00m;
                            }

                            if (salaryAddon.AddonType == SalaryAddonType.Allowance)
                            {
                                totalAllowances += amount;
                            }
                            else if (
                                salaryAddon.AddonType == SalaryAddonType.Deduction || salaryAddon.AddonType == SalaryAddonType.Tax ||
                                salaryAddon.AddonType == SalaryAddonType.SocialSecuritySchemesEmployeeShare)
                            {
                                totalDeductions += amount;
                            }
                            else if (salaryAddon.AddonType == SalaryAddonType.SocialSecuritySchemesCompanyShare)
                            {
                                totalEmployerContributions += amount;
                            }
                        }

                        var monthlyAddon = new EmployeeMonthlySalaryAddon()
                        {
                            SalaryAddonId = addon.SalaryAddonId,
                            AdjustedValue = addon.AdjustedValue,
                            OriginalValue = addon.OriginalValue,
                            Amount = amount,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = currentUserService.UserId,
                            UpdateDate = DateTime.Now,
                            UpdatedByUserId = currentUserService.UserId,
                            IsActive = true
                        };

                        employeeMonthlySalary.EmployeeMonthlySalaryAddons.Add(monthlyAddon);
                    }
                }

                await context.SaveChangesAsync(CancellationToken.None);

                foreach (var taxAddon in employeeTaxSalaryAddons)
                {
                    var deductionRecord = employeeMonthlySalary.EmployeeMonthlySalaryAddons.FirstOrDefault(x => x.Id == taxAddon.Id);

                    var totalEarning = employeeMonthlySalary.BasicSalary + employeeMonthlySalary.EmployeeMonthlySalaryAddons
                         .Where(x => x.SalaryAddon.AddonType == SalaryAddonType.Allowance)
                         .Sum(x => x.Amount);

                    var tax = 0.00m;

                    switch (taxAddon.SalaryAddon.ProportionType)
                    {
                        case ProportionType.FixedAmount:
                            {
                                tax = taxAddon.AdjustedValue;
                            }
                            break;
                        case ProportionType.Percentage:
                            {
                                tax = (totalEarning * taxAddon.AdjustedValue) / 100.00m;
                            }
                            break;
                        case ProportionType.MultipleRange:
                            {
                                tax = CalculateMonthlyTaxForMultipleRange(totalEarning, taxAddon.SalaryAddon.TaxLogics.ToList());
                            }
                            break;
                    }

                    deductionRecord.Amount = tax;
                    totalDeductions += tax;
                }

                //await context.SaveCha                                                                                                                                                                                          ngesAsync(CancellationToken.None);

                employeeMonthlySalary.TotalEarning = employeeMonthlySalary.BasicSalary + totalAllowances;
                employeeMonthlySalary.TotalDeduction = totalDeductions;
                employeeMonthlySalary.EmployerContribution = totalEmployerContributions;
                employeeMonthlySalary.NetSalary = employeeMonthlySalary.TotalEarning - employeeMonthlySalary.TotalDeduction;
                employeeMonthlySalary.UpdateDate = DateTime.Now;
                employeeMonthlySalary.UpdatedByUserId = currentUserService.UserId;
                employeeMonthlySalary.Status = Domain.Enum.EmployeeSalaryStatus.SubmittedForApproval;
                employeeMonthlySalary.IsRevised = true;

                context.EmployeeMonthlySalaries.Update(employeeMonthlySalary);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Employee monthly salary successfully updated." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<GeneralResponseDTO> SubmitMonthlySalaryForApprovalAsBulkAsync(int monthlySalaryId, string comment)
        {
            try
            {
                var monthlySalary = await context.MonthlySalaries
                    .FirstOrDefaultAsync(x => x.Id == monthlySalaryId);
                monthlySalary.Status = MonthlySalaryStatus.SubmittedForApproval;
                monthlySalary.MonthlySalaryComments.Add(new MonthlySalaryComment()
                {
                    Comment = comment,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    IsActive = true
                });

                foreach (var employeeMonthlySalary in monthlySalary.EmployeeMonthlySalaries)
                {
                    employeeMonthlySalary.Status = EmployeeSalaryStatus.Approved;
                    employeeMonthlySalary.UpdateDate = DateTime.Now;
                    employeeMonthlySalary.UpdatedByUserId = currentUserService.UserId;
                }

                context.MonthlySalaries.Update(monthlySalary);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Monthly salaries are submitted for approval successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<GeneralResponseDTO> ApproveMonthlySalaryAsBulkAsync(int monthlySalaryId, string comment)
        {
            try
            {
                var monthlySalary = await context.MonthlySalaries
                    .FirstOrDefaultAsync(x => x.Id == monthlySalaryId);
                monthlySalary.Status = MonthlySalaryStatus.Approved;
                monthlySalary.MonthlySalaryComments.Add(new MonthlySalaryComment()
                {
                    Comment = comment,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    IsActive = true
                });

                foreach (var employeeMonthlySalary in monthlySalary.EmployeeMonthlySalaries)
                {
                    employeeMonthlySalary.Status = EmployeeSalaryStatus.Approved;
                    employeeMonthlySalary.UpdateDate = DateTime.Now;
                    employeeMonthlySalary.UpdatedByUserId = currentUserService.UserId;
                }

                context.MonthlySalaries.Update(monthlySalary);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Monthly salary approved successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<GeneralResponseDTO> AskToReviseMonthlySalaryAsBulkAsync(int monthlySalaryId, string comment)
        {
            try
            {
                var monthlySalary = await context.MonthlySalaries
                    .FirstOrDefaultAsync(x => x.Id == monthlySalaryId);
                monthlySalary.Status = MonthlySalaryStatus.SubmittedForRevised;
                monthlySalary.MonthlySalaryComments.Add(new MonthlySalaryComment()
                {
                    Comment = comment,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    IsActive = true
                });

                foreach (var employeeMonthlySalary in monthlySalary.EmployeeMonthlySalaries)
                {
                    employeeMonthlySalary.Status = EmployeeSalaryStatus.SubmittedForRevised;
                    employeeMonthlySalary.UpdateDate = DateTime.Now;
                    employeeMonthlySalary.UpdatedByUserId = currentUserService.UserId;
                }

                context.MonthlySalaries.Update(monthlySalary);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Monthly salary revision requested successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<GeneralResponseDTO> UpdateMonthlySalarySubmittedToBankAsBulkAsync(int monthlySalaryId, string comment)
        {
            try
            {
                var monthlySalary = await context.MonthlySalaries
                    .FirstOrDefaultAsync(x => x.Id == monthlySalaryId);
                monthlySalary.Status = MonthlySalaryStatus.SubmittedToBank;
                monthlySalary.MonthlySalaryComments.Add(new MonthlySalaryComment()
                {
                    Comment = comment,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    IsActive = true
                });

                context.MonthlySalaries.Update(monthlySalary);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Monthly salary submitted to bank successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<GeneralResponseDTO> UpdateMonthlySalaryTransferredAsBulkAsync(int monthlySalaryId, string comment)
        {
            try
            {
                var monthlySalary = await context.MonthlySalaries
                    .FirstOrDefaultAsync(x => x.Id == monthlySalaryId);
                monthlySalary.Status = MonthlySalaryStatus.Transferred;
                monthlySalary.MonthlySalaryComments.Add(new MonthlySalaryComment()
                {
                    Comment = comment,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    IsActive = true
                });

                context.MonthlySalaries.Update(monthlySalary);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Monthly salary transferred successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }

        public async Task<EmployeeMonthlySalaryDTO> GetEmployeeMonthlySalary(int id)
        {
            var employeeMonthlySalary = await context.EmployeeMonthlySalaries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employeeMonthlySalary == null)
            {
                return null;
            }

            var monthlySalary = new EmployeeMonthlySalaryDTO()
            {
                Id = employeeMonthlySalary.Id,
                EmployeeSalaryId = employeeMonthlySalary.EmployeeSalaryId,
                BasicSalary = employeeMonthlySalary.BasicSalary,
                TotalEarning = employeeMonthlySalary.TotalEarning,
                TotalDeduction = employeeMonthlySalary.TotalDeduction,
                NetSalary = employeeMonthlySalary.NetSalary,
                EmployerContribution = employeeMonthlySalary.EmployerContribution,
                MonthlySalaryId = employeeMonthlySalary.MonthlySalaryId,
                Status = employeeMonthlySalary.Status,
                CreatedDate = employeeMonthlySalary.CreatedDate
            };

            foreach (var addon in employeeMonthlySalary.EmployeeMonthlySalaryAddons)
            {
                //var salaryAddon = await context.SalaryAddons.FirstOrDefaultAsync(x => x.Id == addon.SalaryAddonId);
                var propostionType = addon.SalaryAddon.ProportionType == Domain.Enum.ProportionType.FixedAmount ? "$" : "%";
                monthlySalary.EmployeeSalaryAddons.Add(new EmployeeMonthlySalaryAddonDTO()
                {
                    Id = addon.Id,
                    EmployeeMonthlySalaryId = addon.EmployeeMonthlySalaryId,
                    SalaryAddon = $"{addon.SalaryAddon.Name} ({propostionType})",
                    Amount = addon.Amount,
                    SalaryAddonId = addon.SalaryAddonId,
                    OriginalValue = addon.OriginalValue,
                    AdjustedValue = addon.AdjustedValue,
                    ApplyForAllEmployees = addon.SalaryAddon.ApplyForAllEmployees,
                    IsApplicableForPaye = addon.IsPayeApplicable,
                    ProportionType = addon.SalaryAddon.ProportionType
                });
            }

            return monthlySalary;
        }

        #endregion

        #region Private Methods
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
            if (string.IsNullOrEmpty(replacement))
            {
                replacement = "N/A";
            }
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
        private void FillCellValue(TableRow row, int cellIndex, string value, int fontSize = 10)
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
                var runProperties = new RunProperties(new Bold() { Val = false });
                Run run = new Run(runProperties, new Text(value));
                paragraph.Append(run);

                OpenXMLTableHelper.SetCellFontSize(cell, fontSize);
            }
        }

        private decimal CalculateMonthlyTaxForMultipleRange(decimal monthlySalary, List<TaxLogic> taxLogics)
        {
            decimal tax = 0;

            if (taxLogics == null || taxLogics.Count == 0)
            {
                return tax;
            }

            if (monthlySalary <= taxLogics.FirstOrDefault().MaxSalary)
            {
                return 0;
            }

            var brackets = taxLogics.Skip(1).Select(x => new
            {
                MaxLimit = x.MaxSalary,
                MinLimit = x.MinSalary,
                Rate = x.TaxRate / 100.00m
            }).ToArray();


            for (int i = 0; i < brackets.Length; i++)
            {
                if (monthlySalary >= brackets[i].MinLimit && monthlySalary <= brackets[i].MaxLimit)
                {
                    decimal taxableAmount = monthlySalary - brackets[i].MinLimit;
                    tax += taxableAmount * brackets[i].Rate;
                }
                else if (monthlySalary >= brackets[i].MinLimit && monthlySalary > brackets[i].MaxLimit)
                {
                    decimal taxableAmount = brackets[i].MaxLimit - brackets[i].MinLimit;
                    tax += taxableAmount * brackets[i].Rate;
                }
                else
                {

                }

            }

            return Math.Round(tax, 2);

        }



        #endregion
    }
}
