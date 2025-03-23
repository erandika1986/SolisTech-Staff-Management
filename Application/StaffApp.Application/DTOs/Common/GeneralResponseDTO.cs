using StaffApp.Application.Extensions.Constants;

namespace StaffApp.Application.DTOs.Common
{
    public record GeneralResponseDTO(bool Flag = false, string Message = null!, string UserId = ApplicationConstants.EmptyGuide);
}
