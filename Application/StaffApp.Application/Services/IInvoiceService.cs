using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;
using StaffApp.Domain.Enum;

namespace StaffApp.Application.Services
{
    public interface IInvoiceService
    {
        Task<GeneralResponseDTO> GenerateMonthlyInvoicesAsync(int companyYear, int month);
        Task<PaginatedResultDTO<InvoiceDTO>> GetAllInvoiceAsync(int pageNumber, int pageSize, int companyYear, Month month);
        Task<GeneralResponseDTO> SaveInvoice(InvoiceDTO invoice);
        Task<GeneralResponseDTO> SaveInvoiceDetail(InvoiceDetailDTO invoiceDetail);
        Task<GeneralResponseDTO> DeleteInvoice(int invoiceId);
        Task<InvoiceDTO> GetInvoiceByIdAsync(int expenseId);
        Task<DocumentDTO> DownloadInvoiceAsync(int invoiceId);
        Task<GeneralResponseDTO> EmailInvoiceAsync(int invoiceId);
    }
}
