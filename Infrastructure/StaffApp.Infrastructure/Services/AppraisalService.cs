using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Appraisal;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Extensions.Helpers.OpenXML;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.Reflection;

namespace StaffApp.Infrastructure.Services
{
    public class AppraisalService(IStaffAppDbContext context,
        ICurrentUserService currentUserService,
         IFileDownloadService fileDownloadService,
        IUserService userService,
        ILogger<IAppraisalService> logger) : IAppraisalService
    {
        public async Task<GeneralResponseDTO> GenerateUserRecordsForSelectedAppraisalPeriod(int appraisalPeriodId)
        {
            var response = new GeneralResponseDTO();

            try
            {
                var activeUsers = await userService.GetAllActiveUsersAsync();

                var existingRecords = await context.UserAppraisals.Where(ua => ua.AppraisalPeriodId == appraisalPeriodId).ToListAsync();

                var appraisalCriteria = await context.UserAppraisalCriterias.ToListAsync();

                foreach (var activeUser in activeUsers)
                {
                    if (!existingRecords.Any(er => er.UserId == activeUser.Id))
                    {
                        var newUserAppraisal = new Domain.Entity.UserAppraisal
                        {
                            AppraisalPeriodId = appraisalPeriodId,
                            UserId = activeUser.Id,
                            ReviewerId = null, // Assign a default reviewer if needed
                            OverallRating = 0,
                            Comments = string.Empty,
                            Status = Domain.Enum.AppraisalStatus.Pending,
                            CreatedByUserId = currentUserService.UserId,
                            CreatedDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            UpdatedByUserId = currentUserService.UserId,
                            IsActive = true
                        };

                        foreach (var criteria in appraisalCriteria)
                        {
                            var detail = new Domain.Entity.UserAppraisalDetail
                            {
                                CriteriaID = criteria.Id,
                                Rating = 0,
                                Comment = string.Empty,
                            };

                            newUserAppraisal.UserAppraisalDetails.Add(detail);
                        }

                        context.UserAppraisals.Add(newUserAppraisal);
                    }
                }

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "User appraisal records generated successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GeneralResponseDTO> GenerateAppraisalDataForSelectedCompanyYear(int companyYearId)
        {
            try
            {
                var appraisalPeriod = await context.AppraisalPeriods
                    .FirstOrDefaultAsync(x => x.CompanyYearId == companyYearId);

                var activeEmployees = await context.ApplicationUsers
                    .Where(x => x.IsActive)
                    .ToListAsync(CancellationToken.None);

                var userAppraisalCriterias = await context.UserAppraisalCriterias
                    .Where(x => x.IsActive == true)
                    .ToListAsync(CancellationToken.None);

                if (appraisalPeriod is null)
                {
                    appraisalPeriod = new Domain.Entity.AppraisalPeriod()
                    {
                        AppraisalPeriodName = $"Annual Appraisal({companyYearId})",
                        AppraisalStatus = Domain.Enum.AppraisalStatus.Pending,
                        CompanyYearId = companyYearId,
                        EndDate = new DateTime(companyYearId, 12, 31),
                        StartDate = new DateTime(companyYearId, 1, 1),
                    };

                    foreach (var activeEmployee in activeEmployees)
                    {
                        var userAppraisal = await GenerateNewUserAppraisalRecord(activeEmployee, userAppraisalCriterias);

                        appraisalPeriod.UserAppraisals.Add(userAppraisal);
                    }

                    context.AppraisalPeriods.Add(appraisalPeriod);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO { Flag = true, Message = "User Appraisal Data generated successfully." };
                }
                else
                {
                    foreach (var activeEmployee in activeEmployees)
                    {
                        var userAppraisal = appraisalPeriod.UserAppraisals.FirstOrDefault(x => x.UserId == activeEmployee.Id);
                        if (userAppraisal is null)
                        {
                            userAppraisal = await GenerateNewUserAppraisalRecord(activeEmployee, userAppraisalCriterias);

                            appraisalPeriod.UserAppraisals.Add(userAppraisal);
                        }
                        else
                        {
                            var newUserAppraisalCriterias = (
                                from c in userAppraisalCriterias
                                where !userAppraisal.UserAppraisalDetails.Any(x => x.CriteriaID == c.Id)
                                select c).ToList();

                            foreach (var newCriterias in newUserAppraisalCriterias)
                            {
                                userAppraisal.UserAppraisalDetails.Add(new UserAppraisalDetail()
                                {
                                    Comment = string.Empty,
                                    CriteriaID = newCriterias.Id,
                                    Rating = 0
                                });
                            }

                            var deletedUserAppraisalCriterias = (
                                from d in userAppraisal.UserAppraisalDetails
                                where userAppraisalCriterias.Any(x => x.Id == d.CriteriaID)
                                select d).ToList();

                            foreach (var deleteCriteria in deletedUserAppraisalCriterias)
                            {
                                userAppraisal.UserAppraisalDetails.Remove(deleteCriteria);
                            }
                        }
                    }

                    context.AppraisalPeriods.Update(appraisalPeriod);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO { Flag = true, Message = "User Appraisal Data re-generated successfully." };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding appraisal period for company year  {companyYearId}", companyYearId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while adding the appraisal period."
                };
            }

        }

        public async Task<PaginatedResultDTO<AppraisalPeriodDTO>> GetAppraisalPeriodForSelectedYear(int companyYearId)
        {
            var query = context.CompanyYears.AsQueryable();
            if (companyYearId > 0)
            {
                query = query.Where(x => x.Id == companyYearId);
            }

            var companyYears = await query.ToListAsync();
            var appraisalPeriodList = new List<AppraisalPeriodDTO>();

            foreach (var companyYear in companyYears)
            {
                if (companyYear.AppraisalPeriods.Count == 0)
                {
                    var appraisalPeriod = new AppraisalPeriodDTO()
                    {
                        Id = 0,
                        AppraisalPeriodName = $"Annual Appraisal({companyYear.Id})",
                        AppraisalStatus = "Not Generated",
                        CompanyYearId = companyYear.Id,
                        EndDate = (new DateTime(companyYear.Id, 12, 31)).ToString("MM/dd/yyyy"),
                        StartDate = (new DateTime(companyYear.Id, 1, 1)).ToString("MM/dd/yyyy")
                    };

                    appraisalPeriodList.Add(appraisalPeriod);
                }
                else
                {
                    foreach (var appraisalPeriod in companyYear.AppraisalPeriods)
                    {
                        appraisalPeriodList.Add(new AppraisalPeriodDTO()
                        {
                            Id = appraisalPeriod.Id,
                            AppraisalPeriodName = appraisalPeriod.AppraisalPeriodName,
                            AppraisalStatus = EnumHelper.GetEnumDescription(appraisalPeriod.AppraisalStatus),
                            CompanyYearId = appraisalPeriod.CompanyYearId,
                            EndDate = appraisalPeriod.EndDate.ToString("MM/dd/yyyy"),
                            StartDate = appraisalPeriod.StartDate.ToString("MM/dd/yyyy")
                        });
                    }
                }
            }

            var newResult = new PaginatedResultDTO<AppraisalPeriodDTO>
            {
                Items = appraisalPeriodList,
                TotalItems = 1,
                Page = 1,
                PageSize = 10,
                IsReadOnly = true
            };

            return newResult;
        }

        public async Task<List<UserAppraisalSummaryDTO>> GetMyAssignedAppraisal(int companyYearId)
        {
            var assignedAppraisals = await (from ua in context.UserAppraisals
                                            join ap in context.AppraisalPeriods on ua.AppraisalPeriodId equals ap.Id
                                            join u in context.ApplicationUsers on ua.UserId equals u.Id
                                            join r in context.ApplicationUsers on ua.ReviewerId equals r.Id into reviewers
                                            from reviewer in reviewers.DefaultIfEmpty()
                                            where ap.CompanyYearId == companyYearId && ua.ReviewerId == currentUserService.UserId
                                            select new UserAppraisalSummaryDTO
                                            {
                                                AppraisalPeriod = ap.AppraisalPeriodName,
                                                Id = ua.Id,
                                                Comments = ua.Comments,
                                                OverallRating = ua.OverallRating,
                                                ReviewerName = reviewer != null ? reviewer.FullName : "N/A",
                                                Status = EnumHelper.GetEnumDescription(ua.Status),
                                                UserFullName = u.FullName
                                            }).ToListAsync(CancellationToken.None);

            return assignedAppraisals;
        }

        public async Task<EmployeeAppraisalDTO> GetEmployeeAppraisalById(int userAppraisalId)
        {
            var userAppraisal = await context.UserAppraisals
                .FirstOrDefaultAsync(ua => ua.Id == userAppraisalId);

            var assignedRoles = await userService.GetLoggedInUserAssignedRoles(userAppraisal.UserId);

            var assignedDepartments = await context.EmployeeDepartments
                .Include(u => u.Department)
                .Where(u => u.UserId == userAppraisal.UserId)
                .Select(x => x.Department.Name).ToListAsync();

            if (userAppraisal == null)
                return new EmployeeAppraisalDTO(); ;

            var employeeAppraisal = CreateEmployeeAppraisalDTO(userAppraisal, assignedRoles, assignedDepartments);

            return employeeAppraisal;

        }


        public async Task<List<EmployeeAppraisalDTO>> GetEmployeeAppraisalsByEmployeeId()
        {
            var employeeId = currentUserService.UserId;

            var employeeAppraisals = new List<Application.DTOs.Appraisal.EmployeeAppraisalDTO>();

            var userAppraisals = await context.UserAppraisals
                .Where(ua => ua.UserId == employeeId && ua.Status == Domain.Enum.AppraisalStatus.Completed)
                .ToListAsync(CancellationToken.None);

            var assignedRoles = await userService.GetLoggedInUserAssignedRoles(employeeId);

            var assignedDepartments = await context.EmployeeDepartments
                .Include(u => u.Department)
                .Where(u => u.UserId == employeeId)
                .Select(x => x.Department.Name).ToListAsync();

            foreach (var userAppraisal in userAppraisals)
            {
                var employeeAppraisal = CreateEmployeeAppraisalDTO(userAppraisal, assignedRoles, assignedDepartments);

                employeeAppraisals.Add(employeeAppraisal);
            }
            return employeeAppraisals;
        }

        public async Task<GeneralResponseDTO> SaveUserAppraisal(EmployeeAppraisalDTO userAppraisal)
        {
            try
            {
                var userAppraisalEntity = await context.UserAppraisals
                    .Include(ua => ua.UserAppraisalDetails)
                    .FirstOrDefaultAsync(ua => ua.Id == userAppraisal.Id);

                userAppraisalEntity.Comments = userAppraisal.ManagerComments;
                //userAppraisalEntity.ReviewerId;
                userAppraisalEntity.Status = Domain.Enum.AppraisalStatus.InReview;
                userAppraisalEntity.UpdateDate = DateTime.UtcNow;
                userAppraisalEntity.UpdatedByUserId = currentUserService.UserId;
                userAppraisalEntity.OverallRating = userAppraisal.AppraisalCriteria.Average(x => x.Rating);
                userAppraisalEntity.AreaForDevelopment = userAppraisal.DevelopmentAreas;
                userAppraisalEntity.GoalsForNextPeriod = userAppraisal.Goals;

                foreach (var detailDTO in userAppraisal.AppraisalCriteria)
                {
                    var detailEntity = userAppraisalEntity.UserAppraisalDetails
                        .FirstOrDefault(d => d.Id == detailDTO.Id);
                    if (detailEntity != null)
                    {
                        detailEntity.Rating = detailDTO.Rating;
                        detailEntity.Comment = detailDTO.Comments;
                    }
                }

                context.UserAppraisals.Update(userAppraisalEntity);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "User appraisal saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GeneralResponseDTO> CompleteUserAppraisal(EmployeeAppraisalDTO userAppraisalDetail)
        {
            try
            {
                await SaveUserAppraisal(userAppraisalDetail);

                var userAppraisalEntity = await context.UserAppraisals
                    .FirstOrDefaultAsync(ua => ua.Id == userAppraisalDetail.Id);
                if (userAppraisalEntity == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "User appraisal not found."
                    };
                }
                userAppraisalEntity.Status = Domain.Enum.AppraisalStatus.Completed;
                userAppraisalEntity.UpdateDate = DateTime.UtcNow;
                userAppraisalEntity.UpdatedByUserId = currentUserService.UserId;
                context.UserAppraisals.Update(userAppraisalEntity);
                await context.SaveChangesAsync(CancellationToken.None);
                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "User appraisal completed successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DocumentDTO> GenerateEmployeeAppraisalDocumentAsync(int userAppraisalId)
        {
            string filePath = string.Empty;
            var documentDto = new DocumentDTO();

            try
            {
                var userAppraisal = await GetEmployeeAppraisalById(userAppraisalId);

                var outPutDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

                var templatePath = System.IO.Path.Combine(outPutDirectory, @"WordTemplates\EmployeePerformanceAppraisalReportTemplate.docx");

                var salarySlipPath = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.EmployeeAppraisalDocumentFolderPath);
                // Create a temporary file for the modified document
                string fileName = $"Employee_Performance_Appraisal_{Guid.NewGuid().ToString()}.docx";
                documentDto.FileName = System.IO.Path.ChangeExtension(fileName, ".pdf");
                string tempDocxPath = System.IO.Path.Combine(salarySlipPath.Value, fileName);
                string tempPdfPath = System.IO.Path.Combine(salarySlipPath.Value, System.IO.Path.ChangeExtension(fileName, ".pdf"));

                File.Copy(templatePath, tempDocxPath, true);

                // Modify the document using OpenXML
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(tempDocxPath, true))
                {
                    // Get the main document part
                    MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                    ReplaceText(mainPart, "#companyYear", userAppraisal.CompanyYear);
                    ReplaceText(mainPart, "#employeeNo", userAppraisal.EmployeeId);
                    ReplaceText(mainPart, "#employeeName", userAppraisal.EmployeeName);
                    ReplaceText(mainPart, "#department", userAppraisal.Department);
                    ReplaceText(mainPart, "#position", userAppraisal.Position);
                    ReplaceText(mainPart, "#reviewedBy", userAppraisal.ReviewerName);
                    ReplaceText(mainPart, "#reviewedOn", userAppraisal.ReviewedOn);

                    ReplaceText(mainPart, "#overallRating", userAppraisal.OverallRating.HasValue ? userAppraisal.OverallRating.Value.ToString("F2") : "0.00");
                    ReplaceText(mainPart, "#performanceLevel", GetPerformanceLevel(userAppraisal.OverallRating.Value));

                    ReplaceText(mainPart, "#managerComments", userAppraisal.ManagerComments);
                    ReplaceText(mainPart, "areasForDevelopment", userAppraisal.DevelopmentAreas.Trim());
                    ReplaceText(mainPart, "goalsForNextPeriod", userAppraisal.Goals.Trim());

                    //ReplaceText(mainPart, "NetSalary", salarySlip.NetSalary.ToString("C"));

                    Document doc = mainPart.Document;
                    List<Table> tables = doc.Descendants<Table>().ToList();
                    if (tables.Count > 0)
                    {
                        //Employee Earnings
                        Table earningTable = tables[2];

                        for (int i = 0; i < userAppraisal.AppraisalCriteria.Count(); i++)
                        {
                            var cellColor = GetRatingColor(userAppraisal.AppraisalCriteria[i].Rating);

                            if (i == 0)
                            {
                                DocumentFormat.OpenXml.Wordprocessing.TableRow earningRow = earningTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(2);
                                // Fill values in specific cells
                                FillCellValue(earningRow, 0, userAppraisal.AppraisalCriteria[i].CriteriaName, 10, cellColor);      // First column
                                FillCellValue(earningRow, 1, userAppraisal.AppraisalCriteria[i].Rating.ToString("F2"), 10, cellColor);      // First column
                                FillCellValue(earningRow, 2, userAppraisal.AppraisalCriteria[i].Comments, 10, cellColor);   // Third column
                            }
                            else
                            {
                                TableRow newRow = new TableRow();

                                // Add cells to the new row
                                string[] cellValues = new string[] { userAppraisal.AppraisalCriteria[i].CriteriaName, userAppraisal.AppraisalCriteria[i].Rating.ToString("F2"), userAppraisal.AppraisalCriteria[i].Comments };
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
                                    HighlightTableCell(cell, cellColor);
                                    newRow.Append(cell);
                                    index++;
                                }

                                earningTable.Append(newRow);
                            }
                        }

                        //Deduction
                        //Table deductionTable = tables[6];

                        //for (int i = 0; i < salarySlip.Deductions.Count; i++)
                        //{
                        //    if (i == 0)
                        //    {
                        //        DocumentFormat.OpenXml.Wordprocessing.TableRow deductionRow = deductionTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);
                        //        // Fill values in specific cells
                        //        FillCellValue(deductionRow, 0, salarySlip.Deductions[i].Description);      // First column
                        //        FillCellValue(deductionRow, 1, salarySlip.Deductions[i].Amount.ToString("C"));   // Third column
                        //    }
                        //    else
                        //    {
                        //        TableRow newRow = new TableRow();
                        //        // Add cells to the new row
                        //        string[] cellValues = new string[] { salarySlip.Deductions[i].Description, salarySlip.Deductions[i].Amount.ToString("C") };
                        //        int index = 0;
                        //        foreach (string value in cellValues)
                        //        {
                        //            var runProperties = new RunProperties(new Bold() { Val = false });
                        //            TableCell cell = new TableCell(new Paragraph(new Run(runProperties, new Text(value))));
                        //            OpenXMLTableHelper.SetCellFontSize(cell, 10);
                        //            //OpenXMLTableHelper.RemoveBoldFromTableCell(cell);
                        //            if (index == 1)
                        //            {
                        //                OpenXMLTableHelper.SetHorizontalTextAlignment(cell, JustificationValues.Right);
                        //            }
                        //            newRow.Append(cell);
                        //            index++;
                        //        }
                        //        deductionTable.Append(newRow);
                        //    }
                        //}


                        //Employer Contribution
                        //Table contributionTable = tables[8];
                        //for (int i = 0; i < salarySlip.EmployerContributions.Count; i++)
                        //{
                        //    if (i == 0)
                        //    {
                        //        DocumentFormat.OpenXml.Wordprocessing.TableRow contributionRow = contributionTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(1);
                        //        // Fill values in specific cells
                        //        FillCellValue(contributionRow, 0, salarySlip.EmployerContributions[i].Description);      // First column
                        //        FillCellValue(contributionRow, 1, salarySlip.EmployerContributions[i].Amount.ToString("C"));   // Third column
                        //    }
                        //    else
                        //    {
                        //        TableRow newRow = new TableRow();
                        //        // Add cells to the new row
                        //        string[] cellValues = new string[] { salarySlip.EmployerContributions[i].Description, salarySlip.EmployerContributions[i].Amount.ToString("C") };
                        //        int index = 0;
                        //        foreach (string value in cellValues)
                        //        {
                        //            var runProperties = new RunProperties(new Bold() { Val = false });
                        //            TableCell cell = new TableCell(new Paragraph(new Run(runProperties, new Text(value))));
                        //            OpenXMLTableHelper.SetCellFontSize(cell, 10);
                        //            //OpenXMLTableHelper.RemoveBoldFromTableCell(cell);
                        //            if (index == 1)
                        //            {
                        //                OpenXMLTableHelper.SetHorizontalTextAlignment(cell, JustificationValues.Right);
                        //            }
                        //            newRow.Append(cell);
                        //            index++;
                        //        }
                        //        contributionTable.Append(newRow);
                        //    }
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
                logger.LogError(ex.ToString());
            }

            return documentDto;
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
        private void FillCellValue(TableRow row, int cellIndex, string value, int fontSize = 10, string cellColor = "")
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

                if (!string.IsNullOrEmpty(cellColor))
                    HighlightTableCell(cell, cellColor);

                // Yellow
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

        private async Task<UserAppraisal> GenerateNewUserAppraisalRecord(
            ApplicationUser activeEmployee,
            List<UserAppraisalCriteria> userAppraisalCriterias)
        {
            var assignedDepartments = await context.EmployeeDepartments.Where(x => x.UserId == activeEmployee.Id).ToListAsync(CancellationToken.None);

            var assignedReviewers = assignedDepartments.Select(x => x.Department)
                .Where(x => x.DepartmentHeadId != null)
                .ToList();

            var userAppraisal = new UserAppraisal()
            {
                Comments = string.Empty,
                CreatedByUserId = currentUserService.UserId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                OverallRating = 0,
                Status = Domain.Enum.AppraisalStatus.Pending,
                UpdateDate = DateTime.UtcNow,
                UpdatedByUserId = currentUserService.UserId,
                UserId = activeEmployee.Id,
                ReviewerId = assignedReviewers.Count > 0 ? assignedReviewers.FirstOrDefault().DepartmentHeadId : null
            };

            foreach (var criteria in userAppraisalCriterias)
            {
                userAppraisal.UserAppraisalDetails.Add(new UserAppraisalDetail()
                {
                    Comment = string.Empty,
                    CriteriaID = criteria.Id,
                    Rating = 0
                });
            }

            return userAppraisal;
        }

        private string GetPerformanceLevel(double overallRating)
        {
            return overallRating switch
            {
                >= 9.0 => "Outstanding",
                >= 7.0 => "Good",
                >= 5.0 => "Satisfactory",
                _ => "Needs Improvement"
            };
        }

        private EmployeeAppraisalDTO CreateEmployeeAppraisalDTO(UserAppraisal userAppraisal, List<string> assignedRoles, List<string> assignedDepartments)
        {
            var employeeAppraisal = new Application.DTOs.Appraisal.EmployeeAppraisalDTO();

            employeeAppraisal.Id = userAppraisal.Id;
            employeeAppraisal.AppraisalPeriod = userAppraisal.AppraisalPeriod.AppraisalPeriodName;
            employeeAppraisal.ManagerComments = userAppraisal.Comments;
            employeeAppraisal.DevelopmentAreas = userAppraisal.AreaForDevelopment;
            employeeAppraisal.Goals = userAppraisal.GoalsForNextPeriod;
            employeeAppraisal.EmployeeId = userAppraisal.User.EmployeeNumber.HasValue ? userAppraisal.User.EmployeeNumber.Value.ToString() : string.Empty;
            employeeAppraisal.EmployeeName = userAppraisal.User.FullName;
            employeeAppraisal.Department = assignedDepartments.Count > 0 ? string.Join(',', assignedDepartments) : string.Empty;
            employeeAppraisal.Status = userAppraisal.Status;
            employeeAppraisal.Position = assignedRoles.Count > 0 ? string.Join(',', assignedRoles) : string.Empty;
            employeeAppraisal.DevelopmentAreas = userAppraisal.AreaForDevelopment;
            employeeAppraisal.Goals = userAppraisal.GoalsForNextPeriod;
            employeeAppraisal.ReviewerName = userAppraisal.Reviewer != null ? userAppraisal.Reviewer.FullName : "N/A";
            employeeAppraisal.ReviewedOn = userAppraisal.UpdateDate.HasValue ? userAppraisal.UpdateDate.Value.ToString("MM/dd/yyyy") : string.Empty;
            employeeAppraisal.OverallRating = userAppraisal.OverallRating;
            employeeAppraisal.CompanyYear = userAppraisal.AppraisalPeriod.CompanyYearId.ToString();

            foreach (var detail in userAppraisal.UserAppraisalDetails)
            {
                employeeAppraisal.AppraisalCriteria.Add(new Application.DTOs.Appraisal.AppraisalCriteriaDTO
                {
                    Id = detail.Id,
                    CriteriaName = detail.UserAppraisalCriteria.CriteriaName,
                    AppraisalID = detail.AppraisalID,
                    CriteriaId = detail.CriteriaID,
                    Rating = (double)detail.Rating,
                    Comments = detail.Comment
                });
            }

            return employeeAppraisal;
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

        private void HighlightTableCell(TableCell cell, string hexColor)
        {
            TableCellProperties tcp = cell.GetFirstChild<TableCellProperties>();
            if (tcp == null)
            {
                tcp = new TableCellProperties();
                cell.PrependChild(tcp);
            }

            // Create or update shading
            Shading shading = tcp.GetFirstChild<Shading>();
            if (shading == null)
            {
                shading = new Shading()
                {
                    Val = ShadingPatternValues.Clear, // solid fill
                    Color = "auto",
                    Fill = hexColor   // hex without "#", e.g. "FFFF00" for yellow
                };
                tcp.Append(shading);
            }
            else
            {
                shading.Val = ShadingPatternValues.Clear;
                shading.Color = "auto";
                shading.Fill = hexColor;
            }
        }

        private string GetRatingColor(double rating)
        {
            return rating switch
            {
                >= 9.0 => "28A745",
                >= 7.0 => "17A2B8",
                >= 5.0 => "FFC107",
                _ => "FF0000"
            };
        }
    }
}
