using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
namespace Forum.Services
{
    public interface IMailSender
    {
        void Send(string username, string email, string id, string code);

    }
    public class MailSMTPSender: IMailSender
    {
        private readonly IConfiguration conf;
        private readonly ILogger<MailSMTPSender> _logger;

        public MailSMTPSender(IConfiguration conf, ILogger<MailSMTPSender> logger)
        {
            this.conf = conf;
            _logger = logger;
        }
        public void Send(string username, string email, string id, string code)
        {
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(code);
            code = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            string host = conf["EmailSettings:LinkHost"];
            var verifyUrl = $"{host}/Account/Verify?id={id}&token={code}";
            string subject = "Verify your account";
            var fromMail = new MailAddress(conf["MAIL_LOGIN"], conf["MAIL_LOGIN"]);
            var toMail = new MailAddress(email);
            string body = $"<br/><br/>We are excited to tell you, <strong>{username}</strong>, that your account is" +
         " successfully created. Please click on the below link to verify your account" +
         " <br/><br/><a href='" + verifyUrl + "'>" + verifyUrl + "</a> ";

            var smtp = new SmtpClient
            {
                Host = conf["MAIL_LOGIN"],
                Port = int.Parse(conf["EmailSettings:Port"]),
                EnableSsl = bool.Parse(conf["EmailSettings:EnableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromMail.Address, conf["MAIL_PASSWORD"])
            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            _logger.LogInformation("Sended verification email to {0} to user {1}", email, username);
        }

    }
}
