using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.Services
{
    public class SendMailBackgroundService : ISendMailService
    {
        private readonly MailSettingsDto _mailSettings;
        private readonly ILogger<SendMailBackgroundService> _logger;

        public SendMailBackgroundService(IOptions<MailSettingsDto> mailSettings, ILogger<SendMailBackgroundService> logger)
        {
            this._mailSettings = mailSettings.Value;
            this._logger = logger;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new int[] { 300 })]
        [Queue(HangfireConsts.SEND_MAIL_QUEUE)]
        public void SendMail(MailRequestDto request)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            var builder = new BodyBuilder
            {
                HtmlBody = request.Body
            };
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void Send(MailRequestDto request)
        {
            try
            {
                BackgroundJob.Enqueue(() => this.SendMail(request));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
            }
        }
    }
}
