using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;

namespace StaffApp.Application.Services
{
    public interface IIncomeService
    {
        Task<PaginatedResultDTO<IncomeDTO>> GetAllIncomeAsync(int pageNumber, int pageSize, DateTime startDate, DateTime endDate, int incomeTypeId);
        Task<GeneralResponseDTO> SaveIncome(IncomeDTO income);
        Task<GeneralResponseDTO> DeleteIncome(int incomeId);
        Task<GeneralResponseDTO> DeleteIncomeSupportDocument(int incomeId, int supportDocumentId);
        Task<List<DropDownDTO>> GetIncomeTypes(bool hasDefaultValue = false);
        Task<IncomeDTO> GetIncomeByIdAsync(int expenseId);
    }
}
