﻿using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.CompanySettings;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class CompanySettingService(IStaffAppDbContext context, ICurrentUserService currentUserService) : ICompanySettingService
    {
        public async Task<CompanyDetailDTO> GetCompanyDetail()
        {
            var applicationUrl = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.ApplicationUrl);

            var companyAddress = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.CompanyAddress);

            var companyLogoUrl = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.CompanyLogoUrl);

            var companyName = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.CompanyName);

            var leaveRequestCCList = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.LeaveRequestCCList);

            var isPasswordLoginEnable = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.IsPasswordLoginEnable);

            var companyWebSiteUrl = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.CompanyWebsiteUrl);

            var companyEmail = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.CompanyEmail);

            var companyPhone = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.CompanyPhone);

            return new CompanyDetailDTO()
            {
                ApplicationUrl = applicationUrl.Value,
                CompanyAddress = companyAddress.Value,
                CompanyLogoUrl = companyLogoUrl.Value,
                CompanyName = companyName.Value,
                LeaveRequestCCList = leaveRequestCCList.Value,
                CompanyWebSiteUrl = companyWebSiteUrl.Value,
                CompanyEmail = companyEmail.Value,
                CompanyPhone = companyPhone.Value,
                IsPasswordLoginEnable = bool.Parse(isPasswordLoginEnable.Value)
            };
        }

        public async Task<CompanySMTPSettingDTO> GetCompanySMTPSetting()
        {
            var smtpServer = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SMTPServer);

            var smtpUsername = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SMTPUsername);

            var smtpPassword = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SMTPPassword);
            var smtpPort = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SMTPPort);

            var smtpEnableSsl = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SMTPEnableSsl);

            var smtpSenderEmail = context.AppSettings.FirstOrDefault(x => x.Name == CompanySettingConstants.SMTPSenderEmail);

            return new CompanySMTPSettingDTO()
            {
                SMTPEnableSsl = smtpEnableSsl.Value,
                SMTPPassword = smtpPassword.Value,
                SMTPPort = smtpPort.Value,
                SMTPServer = smtpServer.Value,
                SMTPUsername = smtpUsername.Value,
                SMTPSenderEmail = smtpSenderEmail.Value,

            };

        }

        public async Task<GeneralResponseDTO> SaveCompanyDetailAsync(CompanyDetailDTO companyDetail)
        {
            try
            {
                var applicationUrl = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.ApplicationUrl);
                applicationUrl.Value = companyDetail.ApplicationUrl;

                var companyAddress = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyAddress);
                companyAddress.Value = companyDetail.CompanyAddress;

                var companyLogoUrl = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyLogoUrl);
                companyLogoUrl.Value = companyDetail.CompanyLogoUrl;

                var companyName = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyName);
                companyName.Value = companyDetail.CompanyName;

                var leaveRequestCCList = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.LeaveRequestCCList);
                leaveRequestCCList.Value = companyDetail.LeaveRequestCCList;

                var companyWebSiteUrl = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.CompanyWebsiteUrl);
                companyWebSiteUrl.Value = companyDetail.CompanyWebSiteUrl;

                context.AppSettings.UpdateRange(new List<AppSetting> { applicationUrl, companyAddress, companyLogoUrl, companyName, leaveRequestCCList, companyWebSiteUrl });

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Company settings updated successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }

        }

        public async Task<GeneralResponseDTO> SaveCompanySMTPSettingAsync(CompanySMTPSettingDTO companySMTPSetting)
        {
            try
            {
                var smtpServer = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.SMTPServer);
                smtpServer.Value = companySMTPSetting.SMTPServer;

                var smtpUsername = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.SMTPUsername);
                smtpUsername.Value = companySMTPSetting.SMTPUsername;

                var smtpPassword = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.SMTPPassword);
                smtpPassword.Value = companySMTPSetting.SMTPPassword;

                var smtpPort = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.SMTPPort);
                smtpPort.Value = companySMTPSetting.SMTPPort;

                var smtpEnableSsl = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.SMTPEnableSsl);
                smtpEnableSsl.Value = companySMTPSetting.SMTPEnableSsl;

                var smtpSenderEmail = await context.AppSettings.FirstOrDefaultAsync(x => x.Name == CompanySettingConstants.SMTPSenderEmail);
                smtpSenderEmail.Value = companySMTPSetting.SMTPSenderEmail;

                context.AppSettings.UpdateRange(new List<AppSetting> { smtpServer, smtpUsername, smtpPassword, smtpPort, smtpEnableSsl, smtpSenderEmail });

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Email settings updated successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }
        }
    }
}
