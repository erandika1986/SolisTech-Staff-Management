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
        Task<GeneralResponseDTO> ApproveTimeCard(int timeCardId, string comment);
        Task<GeneralResponseDTO> RejectTimeCard(int timeCardId, string comment);
    }
}
