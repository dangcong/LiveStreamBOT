

using BaseSouce.ViewModels.User;

namespace BaseSouce.Services.Services.SendMail
{
     public interface ISendMailService
    {
        public Task SendEmailConfirmAccountAsync(string callbackUrl, string emailUser);
        public Task SendEmailInfoRegister(RegisterRequestVm model);
        public Task SendEmailForgotPassword(string callbackUrl, string emailUser);
        public Task SendEmailCodeWithdrawal(string code, string emailUser);
        public Task SendEmailRegisterTradeAccount(string tradingId, string password, string email, string leverage);
        public Task SendEmailDeposit(string email, string tradingId, string tradingCode, string type, string amount);
        public Task SendEmailWithdrawal(string email, string tradingId, string type, string amount, string message);
        public Task SendEmailChangeLeverage(string email, string tradingId, string leverage);
        public Task SendEmailDepositSuccess(string email, string tradingId, string tradingCode, string type, string amount);
    }
}
