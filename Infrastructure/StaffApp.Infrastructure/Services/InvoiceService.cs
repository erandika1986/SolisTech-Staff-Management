using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.ApplicationCore;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers.OpenXML;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.Reflection;

namespace StaffApp.Infrastructure.Services
{
    public class InvoiceService(IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        ICompanySettingService companySettingService,
        IAzureBlobService azureBlobService,
        IFileDownloadService fileDownloadService,
        IConfiguration configuration,
        IEmailService emailService,
        IWebHostEnvironment environment,
        ILogger<IInvoiceService> logger) : IInvoiceService
    {
        public async Task<GeneralResponseDTO> DeleteInvoice(int invoiceId)
        {
            try
            {
                var invoice = await context.Invoices.FindAsync(invoiceId);
                if (invoice == null)
                {
                    return new GeneralResponseDTO()
                    {
                        Flag = false,
                        Message = "Invoice not found.",
                        UserId = currentUserService.UserId ?? string.Empty
                    };
                }

                invoice.IsActive = false;
                invoice.UpdatedByUserId = currentUserService.UserId;
                invoice.UpdateDate = DateTime.Now;

                context.Invoices.Update(invoice);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO()
                {
                    Flag = true,
                    Message = "Invoice deleted successfully.",
                    UserId = currentUserService.UserId ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message, UserId = currentUserService.UserId ?? string.Empty };
            }
        }

        public async Task<DocumentDTO> DownloadInvoiceAsync(int invoiceId)
        {
            string filePath = string.Empty;
            var documentDto = new DocumentDTO();

            try
            {
                var invoice = await context.Invoices
                    .FirstOrDefaultAsync(i => i.Id == invoiceId && i.IsActive);

                var companySettings = await companySettingService.GetCompanyDetail();

                var outPutDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                var templatePath = System.IO.Path.Combine(outPutDirectory, @"WordTemplates\InvoiceTemplate.docx");


                var invoiceFolderPath = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.InvoiceFolderPath);
                if (!Directory.Exists(invoiceFolderPath.Value))
                {
                    // Create the directory
                    Directory.CreateDirectory(invoiceFolderPath.Value);
                }

                string fileName = $"Invoice_{Guid.NewGuid().ToString()}.docx";
                documentDto.FileName = System.IO.Path.ChangeExtension(fileName, ".pdf");
                string tempDocxPath = System.IO.Path.Combine(invoiceFolderPath.Value, fileName);
                string tempPdfPath = System.IO.Path.Combine(invoiceFolderPath.Value, System.IO.Path.ChangeExtension(fileName, ".pdf"));

                File.Copy(templatePath, tempDocxPath, true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(tempDocxPath, true))
                {
                    MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                    ReplaceText(mainPart, "CompanyName", companySettings.CompanyName);
                    ReplaceText(mainPart, "CompanyAddress", companySettings.CompanyAddress);
                    ReplaceText(mainPart, "CompanyEmail", companySettings.CompanyEmail);
                    ReplaceText(mainPart, "CompanyWebSite", companySettings.CompanyWebSiteUrl);
                    ReplaceText(mainPart, "CompanyPhone", companySettings.CompanyPhone);


                    ReplaceText(mainPart, "ClientName", invoice.Project.ClientName);
                    ReplaceText(mainPart, "ClientAddress", invoice.Project.ClientAddress);
                    ReplaceText(mainPart, "ClientEmail", invoice.Project.ClientEmail);
                    ReplaceText(mainPart, "ClientPhone", invoice.Project.ClientPhone);

                    ReplaceText(mainPart, "InvoiceNumber", invoice.InvoiceNumber);
                    ReplaceText(mainPart, "InvoiceDate", invoice.InvoiceDate.ToString("MM/dd/yyyy"));
                    ReplaceText(mainPart, "InvoiceDueDate", invoice.InvoiceDate.AddMonths(1).ToString("MM/dd/yyyy"));

                    ReplaceText(mainPart, "InvoiceSubTotal", invoice.TotalAmount.ToString("F2"));
                    ReplaceText(mainPart, "InvoiceDiscount", 0.0m.ToString("F2"));
                    ReplaceText(mainPart, "InvoiceSubTotalLessDiscount", invoice.TotalAmount.ToString("F2"));
                    ReplaceText(mainPart, "InvoiceTaxRate", 0.0m.ToString("F2"));
                    ReplaceText(mainPart, "InvoiceTotalTax", 0.0m.ToString("F2"));
                    ReplaceText(mainPart, "invoiceBalanceDue", invoice.TotalAmount.ToString("F2"));

                    Document doc = mainPart.Document;
                    List<Table> tables = doc.Descendants<Table>().ToList();

                    //Employee Earnings
                    Table invoiceDetailTable = tables[3];

                    var invoiceDetails = invoice.InvoiceDetails.ToList();

                    for (int i = 0; i < invoiceDetails.Count; i++)
                    {
                        if (i == 0)
                        {
                            DocumentFormat.OpenXml.Wordprocessing.TableRow earningRow = invoiceDetailTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);
                            // Fill values in specific cells
                            FillCellValue(earningRow, 0, invoiceDetails[i].Description);      // First column
                            FillCellValue(earningRow, 1, invoiceDetails[i].Amount.ToString("C"));   // Third column
                        }
                        else
                        {
                            TableRow newRow = new TableRow();

                            // Add cells to the new row
                            string[] cellValues = new string[] { invoiceDetails[i].Description, invoiceDetails[i].Amount.ToString("C") };
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

                            invoiceDetailTable.Append(newRow);
                        }
                    }

                    mainPart.Document.Save();
                }

                // Convert the modified document to PDF
                ConvertWordToPdf(tempDocxPath, tempPdfPath);

                documentDto.FileArray = await fileDownloadService.GetFileAsync(tempPdfPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error downloading invoice with ID {InvoiceId}", invoiceId);
            }

            return documentDto;
        }

        public async Task<GeneralResponseDTO> EmailInvoiceAsync(int invoiceId)
        {
            try
            {
                var invoice = await context.Invoices
                    .Include(i => i.Project)
                    .FirstOrDefaultAsync(i => i.Id == invoiceId);

                var companySettings = await companySettingService.GetCompanyDetail();

                var invoiceAttachment = await DownloadInvoiceAsync(invoiceId);

                if (invoiceAttachment.FileArray == null || invoiceAttachment.FileArray.Length == 0)
                {
                    return new GeneralResponseDTO()
                    {
                        Flag = false,
                        Message = "Invoice file is empty or not found.",
                        UserId = currentUserService.UserId ?? string.Empty
                    };
                }

                string templatePath = Path.Combine(environment.ContentRootPath, "EmailTemplates", "InvoiceEmailTemplate.html");
                string htmlBody = File.ReadAllText(templatePath);

                // Replace placeholders with actual values
                htmlBody = htmlBody
                    .Replace("@CustomerName", invoice.Project.ClientName)
                    .Replace("@InvoiceNumber", invoice.InvoiceNumber)
                    .Replace("@InvoiceDate", invoice.InvoiceDate.ToString("MM/dd/yyyy"))
                    .Replace("@DueDate", invoice.InvoiceDate.AddMonths(1).ToString("MM/dd/yyyy"))
                    .Replace("@TotalAmount", invoice.TotalAmount.ToString("F2"))
                    .Replace("@CompanyName", companySettings.CompanyName)
                    .Replace("@PhoneNumber", companySettings.CompanyPhone)
                    .Replace("@EmailAddress", companySettings.CompanyEmail)
                    .Replace("@WebsiteURL", companySettings.CompanyWebSiteUrl)
                    .Replace("@CompanyAddress", companySettings.CompanyAddress)
                    .Replace("@BankDetails", "");


                List<EmailAttachmentDTO> emailAttachments = new List<EmailAttachmentDTO>();

                emailAttachments.Add(new EmailAttachmentDTO
                {
                    FileName = invoiceAttachment.FileName,
                    Content = invoiceAttachment.FileArray,
                    ContentType = "application/pdf"
                });


                // Send email with attachments
                await emailService.SendEmailWithAttachmentsAsync(
                    invoice.Project.ClientEmail,
                    $"Billing Invoice From {companySettings.CompanyName}",
                    htmlBody,
                    emailAttachments);

                return new GeneralResponseDTO()
                {
                    Flag = true,
                    Message = "Invoice emailed successfully.",
                    UserId = currentUserService.UserId ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error emailing invoice with ID {InvoiceId}", invoiceId);
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message, UserId = currentUserService.UserId ?? string.Empty };
            }
        }
        public async Task<GeneralResponseDTO> GenerateMonthlyInvoicesAsync(int companyYear, int month)
        {
            try
            {
                var projects = await context.Projects.ToListAsync(CancellationToken.None);

                var startDate = new DateTime(companyYear, month, 1);

                var endDate = startDate.AddMonths(1).AddDays(-1);

                foreach (var project in projects)
                {
                    var projectTimeCardEntries = await context.TimeCardEntries
                        .Include(tc => tc.TimeCard)
                        .Include(tc => tc.TimeCard.Employee)
                        .Where(tc => tc.ProjectId == project.Id && tc.TimeCard.Date >= startDate && tc.TimeCard.Date <= endDate && tc.Status == Domain.Enum.TimeCardEntryStatus.Approved)
                        .ToListAsync(CancellationToken.None);

                    var userTimeCards = projectTimeCardEntries.GroupBy(tc => tc.TimeCard.EmployeeID)
                        .Select(g => new
                        {
                            EmployeeId = g.Key,
                            EmployeeName = g.FirstOrDefault().TimeCard.Employee.FullName,
                            TotalHours = g.Sum(tc => tc.HoursWorked)
                        }).ToList();

                    if (!projectTimeCardEntries.Any())
                    {
                        continue; // Skip if no time cards found for the project in the specified period
                    }
                    else
                    {
                        var projectInvoice = await
                            context.Invoices
                            .Where(i => i.ProjectId == project.Id && i.CompanyYearId == companyYear && i.Month == (Domain.Enum.Month)month && i.IsActive)
                            .FirstOrDefaultAsync(CancellationToken.None);
                        if (projectInvoice == null)
                        {
                            projectInvoice = new Domain.Entity.Invoice
                            {
                                ProjectId = project.Id,
                                CompanyYearId = companyYear,
                                InvoiceNumber = await GenerateInvoiceNumberAsync(DateTime.Now),
                                Month = (Domain.Enum.Month)month,
                                InvoiceDate = DateTime.Now,
                                PeriodStart = startDate,
                                PeriodEnd = endDate,
                                CreatedByUserId = currentUserService.UserId,
                                CreatedDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                UpdatedByUserId = currentUserService.UserId,
                                IsActive = true,
                                TotalHours = (decimal)userTimeCards.Sum(u => u.TotalHours),
                                Notes = $"Invoice for project {project.Name} for the period {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}."
                            };

                            var totalAmount = 0.0m;

                            foreach (var user in userTimeCards)
                            {
                                var projectMember = await context.ProjectMembers
                                    .FirstOrDefaultAsync(pm => pm.ProjectId == project.Id && pm.MemberId == user.EmployeeId && pm.IsActive);

                                var hourlyRate = projectMember.HourlyRate.HasValue ? projectMember.HourlyRate.Value : 0.0m;
                                var invoiceDetail = new Domain.Entity.InvoiceDetail
                                {
                                    Invoice = projectInvoice,
                                    Description = $"{user.EmployeeName} - {user.TotalHours} Hours x {hourlyRate} $",
                                    Amount = (decimal)user.TotalHours * hourlyRate,
                                    CreatedByUserId = currentUserService.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedByUserId = currentUserService.UserId,
                                    UpdateDate = DateTime.Now,
                                    IsActive = true,
                                    EmployeeId = user.EmployeeId,
                                    TotalHours = (decimal)user.TotalHours

                                };

                                totalAmount += invoiceDetail.Amount;
                                projectInvoice.InvoiceDetails.Add(invoiceDetail);
                            }

                            projectInvoice.TotalAmount = totalAmount;

                            context.Invoices.Add(projectInvoice);
                            await context.SaveChangesAsync(CancellationToken.None);
                        }
                        else
                        {
                            var totalAmount = 0.0m;
                            foreach (var userTimeCard in userTimeCards)
                            {
                                var projectMember = await context.ProjectMembers
                                    .FirstOrDefaultAsync(pm => pm.ProjectId == project.Id && pm.MemberId == userTimeCard.EmployeeId && pm.IsActive);

                                var hourlyRate = projectMember.HourlyRate.HasValue ? projectMember.HourlyRate.Value : 0.0m;

                                var invoiceDetail = projectInvoice.InvoiceDetails
                                    .FirstOrDefault(id => id.EmployeeId == userTimeCard.EmployeeId);

                                if (invoiceDetail == null)
                                {
                                    invoiceDetail = new Domain.Entity.InvoiceDetail
                                    {
                                        Invoice = projectInvoice,
                                        Description = $"{userTimeCard.EmployeeName} - {userTimeCard.TotalHours} Hours x {hourlyRate} $",
                                        Amount = (decimal)userTimeCard.TotalHours * hourlyRate, // Assuming HourlyRate is a property of Project
                                        CreatedByUserId = currentUserService.UserId,
                                        CreatedDate = DateTime.Now,
                                        UpdatedByUserId = currentUserService.UserId,
                                        UpdateDate = DateTime.Now,
                                        IsActive = true,
                                        EmployeeId = userTimeCard.EmployeeId
                                    };

                                    totalAmount += invoiceDetail.Amount;
                                    projectInvoice.InvoiceDetails.Add(invoiceDetail);
                                }
                                else
                                {
                                    // Update existing invoice detail if it already exists
                                    invoiceDetail.Description = $"{userTimeCard.EmployeeName} - {userTimeCard.TotalHours} Hours x {hourlyRate} $";
                                    invoiceDetail.Amount = (decimal)userTimeCard.TotalHours * hourlyRate; // Update amount
                                    totalAmount += invoiceDetail.Amount;
                                }
                            }

                            projectInvoice.TotalAmount = totalAmount;

                            projectInvoice.UpdatedByUserId = currentUserService.UserId;
                            projectInvoice.UpdateDate = DateTime.Now;

                            context.Invoices.Update(projectInvoice);
                            await context.SaveChangesAsync(CancellationToken.None);
                        }
                    }
                }

                return new GeneralResponseDTO()
                {
                    Flag = true,
                    Message = "Monthly invoices generated successfully.",
                    UserId = currentUserService.UserId ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message, UserId = currentUserService.UserId ?? string.Empty };
            }

        }

        public async Task<PaginatedResultDTO<InvoiceDTO>> GetAllInvoiceAsync(int pageNumber, int pageSize, int companyYear, Domain.Enum.Month month)
        {
            var query = context.Invoices
                .Where(i => i.CompanyYearId == companyYear && i.Month == month && i.IsActive);

            var totalCount = await query.CountAsync();

            var invoices = await query.OrderByDescending(i => i.InvoiceDate).Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new InvoiceDTO
            {
                Id = i.Id,
                ProjectName = i.ProjectId.HasValue ? i.Project.Name : string.Empty,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate.ToString("yyyy-MM-dd"),
                TotalAmount = i.TotalAmount,
                StartDate = i.PeriodStart.ToString("yyyy-MM-dd"),
                EndDate = i.PeriodEnd.ToString("yyyy-MM-dd"),
                TotalHours = i.TotalHours.HasValue ? i.TotalHours.Value : 0.0m,
                InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                {
                    Id = d.Id,
                    InvoiceId = d.InvoiceId,
                    Description = d.Description,
                    Amount = d.Amount,
                    TotalHours = d.TotalHours.HasValue ? d.TotalHours.Value : 0.0m,
                }).ToList()
            }).ToListAsync();

            var newResult = new PaginatedResultDTO<InvoiceDTO>
            {
                Items = invoices,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize,
                IsReadOnly = true
            };

            return newResult;
        }

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.IsActive);

            var invoiceDTO = invoice == null ? null : new InvoiceDTO
            {
                Id = invoice.Id,
                ProjectName = invoice.ProjectId.HasValue ? invoice.Project.Name : string.Empty,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                TotalAmount = invoice.TotalAmount,
                StartDate = invoice.PeriodStart.ToString("yyyy-MM-dd"),
                EndDate = invoice.PeriodEnd.ToString("yyyy-MM-dd"),
                TotalHours = invoice.TotalHours.HasValue ? invoice.TotalHours.Value : 0.0m,
                InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetailDTO
                {
                    Id = d.Id,
                    InvoiceId = d.InvoiceId,
                    EmployeeId = d.EmployeeId,
                    Description = d.Description,
                    Amount = d.Amount,
                    TotalHours = d.TotalHours.HasValue ? d.TotalHours.Value : 0.0m,
                }).ToList()
            };

            return invoiceDTO;
        }

        public async Task<GeneralResponseDTO> SaveInvoice(InvoiceDTO invoice)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponseDTO> SaveInvoiceDetail(InvoiceDetailDTO invoiceDetailDTO)
        {
            try
            {
                var invoice = await context.Invoices.FindAsync(invoiceDetailDTO.InvoiceId);

                var invoiceDetail = invoice.InvoiceDetails.FirstOrDefault(x => x.Id == invoiceDetailDTO.Id);
                if (invoiceDetail is null)
                {
                    invoice.InvoiceDetails.Add(new Domain.Entity.InvoiceDetail()
                    {
                        Description = invoiceDetailDTO.Description,
                        Amount = invoiceDetailDTO.Amount,
                        EmployeeId = invoiceDetailDTO.EmployeeId,
                        InvoiceId = invoiceDetailDTO.InvoiceId,
                        TotalHours = invoiceDetailDTO.TotalHours,
                        CreatedByUserId = currentUserService.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedByUserId = currentUserService.UserId,
                        UpdateDate = DateTime.Now,
                        IsActive = true
                    });
                }
                else
                {
                    invoiceDetail.Amount = invoiceDetailDTO.Amount;
                    invoiceDetail.TotalHours = invoiceDetailDTO.TotalHours;
                    invoiceDetail.Description = invoiceDetailDTO.Description;
                    invoiceDetail.UpdateDate = DateTime.Now;
                    invoiceDetail.UpdatedByUserId = currentUserService.UserId;
                }

                invoice.TotalHours = invoice.InvoiceDetails.Sum(x => x.TotalHours);
                invoice.TotalAmount = invoice.InvoiceDetails.Sum(x => x.Amount);
                invoice.UpdateDate = DateTime.Now;
                invoice.UpdatedByUserId = currentUserService.UserId;

                context.Invoices.Update(invoice);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO()
                {
                    Flag = true,
                    Message = "Invoice detail successfully saved ."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());

                return new GeneralResponseDTO()
                {
                    Flag = false,
                    Message = "An Error has been occurred. Please try again."
                };
            }
        }

        private async Task<string> GenerateInvoiceNumberAsync(DateTime date)
        {
            string datePart = date.ToString("yyyyMMdd");
            string prefix = $"INV{datePart}";

            // Get the max sequence number for today's invoices
            var lastInvoice = await context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;

            if (lastInvoice != null)
            {
                string lastNumber = lastInvoice.InvoiceNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumber, out int lastSeq))
                {
                    nextSequence = lastSeq + 1;
                }
            }

            string invoiceNumber = $"{prefix}{nextSequence.ToString("D4")}";
            return invoiceNumber;
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


    }
}
