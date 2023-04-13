using BaseSouce.ViewModels.User;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.SendMail
{
    public class SendMailService : ISendMailService
    {
        private readonly ILogger<SendMailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _keyPublicToken;
        private readonly string _keyPrivateToken;
        public SendMailService(ILogger<SendMailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _keyPublicToken = _configuration["mailJet:publicKey"] ?? "8719b8250344e7f54cbc3fbd26135e34";
            _keyPrivateToken = _configuration["mailJet:privateKey"] ?? "cfef67ace7375f7000daf70613af14ed";
        }

        public async Task SendEmailChangeLeverage(string email, string tradingId, string leverage)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                var mailAdmin = _configuration["mailJet:mailAdmin"] ?? string.Empty;
                var mailAdmin2 = _configuration["mailJet:mailAdmin2"] ?? string.Empty;
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateChageLeverage.html"));
                body = body.Replace("{{Email}}", email)
                    .Replace("{{TradeId}}", tradingId)
                    .Replace("{{Leverage}}", leverage)
                    .Replace("{{Currency}}", "USD")
                    .Replace("{{CreateDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Thông tin thay đổi đòn bẩy tài khoản MT5 của người dùng!")
                   .Property(Send.Subject, "【SHARON BROKER】Thông tin thay đổi đòn bẩy tài khoản MT5 của người dùng!")
                   .Property(Send.TextPart, "【SHARON BROKER】Thông tin thay đổi đòn bẩy tài khoản MT5 của người dùng!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, email)
                   .Property(Send.Cc, mailAdmin)
                   .Property(Send.Bcc, mailAdmin2);
                await client.PostAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail info change leverage account mt5 failed by error: [{ex}]");
            }
        }

        public async Task SendEmailCodeWithdrawal(string code, string emailUser)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                // Plug in your email service here to send an email.using (MailMessage mail = new MailMessage(userMailReceiver[0].EmailSender, message.Destination)
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateEmailConfirmWithdrawal.html"));
                body = body.Replace("{{Code}}", code);

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Confirm Withdrawal account MT5")
                   .Property(Send.Subject, "【SHARON BROKER】Confirm Withdrawal account MT5")
                   .Property(Send.TextPart, "【SHARON BROKER】Confirm Withdrawal account MT5")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, emailUser);

                await client.PostAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail confirm withdrawal account MT4 failed by error: [{ex}]");

            }
        }

        public async Task SendEmailConfirmAccountAsync(string callbackUrl, string emailUser)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                // Plug in your email service here to send an email.using (MailMessage mail = new MailMessage(userMailReceiver[0].EmailSender, message.Destination)
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateEmailConfirmRegister.html"));
                body = body.Replace("{{Link}}", callbackUrl);

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Confirm Your Registration")
                   .Property(Send.Subject, "【SHARON BROKER】Confirm Your Registration!")
                   .Property(Send.TextPart, "【SHARON BROKER】Confirm Your Registration!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, emailUser);

                await client.PostAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail confirm create account failed by error: [{ex}]");

            }
        }

        public async Task SendEmailDeposit(string email, string tradingId, string tradingCode, string type, string amount)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                var mailAdmin = _configuration["mailJet:mailAdmin"] ?? string.Empty;
                var mailAdmin2 = _configuration["mailJet:mailAdmin2"] ?? string.Empty;

                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateDepositEmail.html"));
                body = body.Replace("{{Email}}", email)
                    .Replace("{{AccountId}}", tradingId)
                    .Replace("{{TradingCode}}", tradingCode)
                    .Replace("{{Type}}", type)
                    .Replace("{{Total}}", $"{amount} USD")
                    .Replace("{{CreateDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Thông tin nạp tiền tài khoản MT5 của người dùng!")
                   .Property(Send.Subject, "【SHARON BROKER】Thông tin nạp tiền tài khoản MT5 của người dùng!")
                   .Property(Send.TextPart, "【SHARON BROKER】Thông tin nạp tiền tài khoản MT5 của người dùng!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, email)
                   .Property(Send.Cc, mailAdmin)
                   .Property(Send.Bcc, mailAdmin2);
                await client.PostAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail info deposit account failed by error: [{ex}]");
            }
        }

        public async Task SendEmailDepositSuccess(string email, string tradingId, string tradingCode, string type, string amount)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                var mailAdmin = _configuration["mailJet:mailAdmin"] ?? string.Empty;
                var mailAdmin2 = _configuration["mailJet:mailAdmin2"] ?? string.Empty;

                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateDepositEmailSuccess.html"));
                body = body.Replace("{{Email}}", email)
                    .Replace("{{AccountId}}", tradingId)
                    .Replace("{{TradingCode}}", tradingCode)
                    .Replace("{{Type}}", type)
                    .Replace("{{Total}}", $"{amount} USD")
                    .Replace("{{CreateDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Thông tin nạp tiền thành công!")
                   .Property(Send.Subject, "【SHARON BROKER】Thông tin nạp tiền thành công!")
                   .Property(Send.TextPart, "【SHARON BROKER】Thông tin nạp tiền thành công!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, email)
                   .Property(Send.Cc, mailAdmin)
                   .Property(Send.Bcc, mailAdmin2);
                await client.PostAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail info deposit account success, failed by error: [{ex}]");
            }
        }

        public async Task SendEmailForgotPassword(string callbackUrl, string emailUser)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                // Plug in your email service here to send an email.using (MailMessage mail = new MailMessage(userMailReceiver[0].EmailSender, message.Destination)
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateEmailConfirmResetPassword.html"));
                body = body.Replace("{{Link}}", callbackUrl);

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Reset password account!")
                   .Property(Send.Subject, "【SHARON BROKER】Reset password account!")
                   .Property(Send.TextPart, "【SHARON BROKER】Reset password account!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, emailUser);

                await client.PostAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail confirm create account failed by error: [{ex}]");

            }
        }

        public async Task SendEmailInfoRegister(RegisterRequestVm model)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                var mailAdmin = _configuration["mailJet:mailAdmin"] ?? string.Empty;
                var mailAdmin2 = _configuration["mailJet:mailAdmin2"] ?? string.Empty;
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateEmail.html"));
                //body = body.Replace("{{Email}}", model.Email).Replace("{{Address}}", model.CountryName).Replace("{{Phone}}", model.PhoneNumber).Replace("{{FullName}}", $"{model.FirstName} {model.LastName}")
                //    .Replace("{{CreateDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Thông tin đăng ký tài khoản của người dùng!")
                   .Property(Send.Subject, "【SHARON BROKER】Thông tin đăng ký tài khoản của người dùng!")
                   .Property(Send.TextPart, "【SHARON BROKER】Thông tin đăng ký tài khoản của người dùng!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, mailAdmin)
                   //.Property(Send.Cc, mailAdmin)
                   .Property(Send.Bcc, mailAdmin2);
                await client.PostAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail info create account failed by error: [{ex}]");
            }
        }

      

        public async Task SendEmailRegisterTradeAccount(string tradingId, string password, string email, string leverage)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                var mailAdmin = _configuration["mailJet:mailAdmin"] ?? string.Empty;
                var mailAdmin2 = _configuration["mailJet:mailAdmin2"] ?? string.Empty;
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateRegisterTradeAccountEmail.html"));
                body = body.Replace("{{Email}}", email)
                    .Replace("{{TradeId}}", tradingId)
                    .Replace("{{Password}}", password)
                    .Replace("{{Leverage}}", leverage)
                    .Replace("{{Currency}}", "USD")
                    .Replace("{{CreateDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Thông tin đăng ký tài khoản MT5 của người dùng!")
                   .Property(Send.Subject, "【SHARON BROKER】Thông tin đăng ký tài khoản MT5 của người dùng!")
                   .Property(Send.TextPart, "【SHARON BROKER】Thông tin đăng ký tài khoản MT5 của người dùng!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, email)
                   .Property(Send.Cc, mailAdmin)
                   .Property(Send.Bcc, mailAdmin2);
                await client.PostAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail info create account mt4 failed by error: [{ex}]");
            }
        }

        public async Task SendEmailWithdrawal(string email, string tradingId, string type, string amount, string message)
        {
            try
            {
                var mailSender = _configuration["mailJet:mailSender"] ?? string.Empty;
                var mailAdmin = _configuration["mailJet:mailAdmin"] ?? string.Empty;
                var mailAdmin2 = _configuration["mailJet:mailAdmin2"] ?? string.Empty;
                var body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "TemplateWithdrawaltEmai.html"));
                body = body.Replace("{{Email}}", email)
                    .Replace("{{AccountId}}", tradingId)
                    .Replace("{{Message}}", message)
                    .Replace("{{Type}}", type)
                    .Replace("{{Total}}", $"{amount} USD")
                    .Replace("{{CreateDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                MailjetClient client = new MailjetClient(_keyPublicToken, _keyPrivateToken);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, mailSender)
                   .Property(Send.FromName, "【SHARON BROKER】Thông tin rút tiền tài khoản MT5 của người dùng!")
                   .Property(Send.Subject, "【SHARON BROKER】Thông tin rút tiền tài khoản MT5 của người dùng!")
                   .Property(Send.TextPart, "【SHARON BROKER】Thông tin rút tiền tài khoản MT5 của người dùng!")
                   .Property(Send.HtmlPart, body)
                   .Property(Send.To, email)
                   .Property(Send.Cc, mailAdmin)
                   .Property(Send.Bcc, mailAdmin2);
                await client.PostAsync(request);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send mail info withdrawal account failed by error: [{ex}]");
            }
        }
    }
}
