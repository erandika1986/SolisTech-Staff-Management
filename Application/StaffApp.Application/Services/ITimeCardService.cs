using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.TimeCard;

namespace StaffApp.Application.Services
{
    public interface ITimeCardService
    {
        Task<GeneralResponseDTO> SaveTimeCardAsync(TimeCardDTO timeCard);
        Task<TimeCardDTO> GetTimeCardByIdAsync(int timeCardId);
        Task<TimeCardDTO> GetTimeCardByDateAsync(DateTime date);
        Task<GeneralResponseDTO> DeleteTimeCardAsync(int timeCardId);
        Task<PaginatedResultDTO<BasicTimeCardDTO>> GetAllTimeCardAsync(
            int pageNumber,
            int pageSize,
            DateTime fromDate,
            DateTime toDate);

        Task<PaginatedResultDTO<BasicManagerTimeCardDTO>> GetMyEmployeeTimeCardsForSelectedDateAsync(
            int pageNumber,
            int pageSize,
            DateTime timeCardDate);

        Task<GeneralResponseDTO> ApproveTimeCard(int timeCardId, int timeCardEntryId, string comment);
        Task<GeneralResponseDTO> RejectTimeCard(int timeCardId, int timeCardEntryId, string comment);
        Task<GeneralResponseDTO> GenerateTimeCardForSelectedMonth(int companyYear, int companyMonth);
        Task<GeneralResponseDTO> SetTimeCardOnHoliday(string employeeId, DateTime startDate, DateTime endDate);
        Task<PaginatedResultDTO<MonthlyTimeCardDTO>> GetTimeCardsByMonthAsync(
            int pageNumber,
            int pageSize,
            int CompanyYearId,
            int MonthId);
    }
}
