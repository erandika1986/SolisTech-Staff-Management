using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Finance;

namespace StaffApp.Application.Services
{
    public interface IExpenseService
    {
        Task<PaginatedResultDTO<ExpenseDTO>> GetAllExpensesAsync(int pageNumber, int pageSize, DateTime startDate, DateTime endDate, int expenseTypeId);
        Task<GeneralResponseDTO> SaveExpense(ExpenseDTO expense);
        Task<GeneralResponseDTO> DeleteExpense(int expenseId);
        Task<GeneralResponseDTO> DeleteExpenseSupportDocument(int expenseId, int supportDocumentId);
        Task<List<DropDownDTO>> GetExpenseTypes(bool hasDefaultValue = false);
        Task<ExpenseDTO> GetExpenseByIdAsync(int expenseId);

    }
}
