using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class InvoiceService(IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        IConfiguration configuration,
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
    }
}
