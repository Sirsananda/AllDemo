using System.Runtime.InteropServices.Marshalling;
using AllDemo.Configuration;
using AllDemo.Helper.Log;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AllDemo.Services
{
    public class EmailService
    {
        private readonly MailSetting _mailSetting;
        private readonly ErrorLog _errorLog;
        public EmailService(IOptions<MailSetting> options, ErrorLog errorLog)
        {
            _mailSetting = options.Value;
            _errorLog = errorLog;
        }
        public async Task<bool> SendEmail(string subject,string bodyMessage,IFormFile? singleFile,List<IFormFile>? multipleFiles)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSetting.SMTPFrom));
                email.To.Add(MailboxAddress.Parse(_mailSetting.SMTPTo));
                email.Cc.Add(MailboxAddress.Parse(_mailSetting.SMTPCc));
                email.Subject = subject;
                var builder = new BodyBuilder
                {
                    TextBody = bodyMessage
                };
                if (singleFile != null && singleFile.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await singleFile.CopyToAsync(stream);
                    builder.Attachments.Add(singleFile.FileName, stream.ToArray());
                }
                if (multipleFiles != null && multipleFiles.Any())
                {
                    foreach (var file in multipleFiles)
                    {
                        if (file.Length>0)
                        {
                            using var stream = new MemoryStream();
                            await file.CopyToAsync(stream);
                            builder.Attachments.Add(file.FileName, stream.ToArray());
                        }
                    }
                }

                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSetting.SMTPHost, _mailSetting.SMTPPort, SecureSocketOptions.StartTls);
                if (_mailSetting.SMTPAuthentication)
                {
                    await smtp.AuthenticateAsync(_mailSetting.SMTPUser, _mailSetting.SMTPPassword);
                }

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex,"Mail Send failed, Error generate at SendEmail()");
                return false;
            }
        }
    }
}
