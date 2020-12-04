using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;


namespace Forum.Services
{
    public interface IMailSender
    {
        void SendMail(string email, string message, string subject);
    }
    public class MailSMTPSender: IMailSender
    {
        private IConfiguration conf;
        public MailSMTPSender(IConfiguration conf)
        {
            this.conf = conf;
        }
        public void SendMail(string email, string body, string subject)
        {
            var fromMail = new MailAddress(conf["EmailSettings:Login"], conf["EmailSettings:Login"]);
            var toMail = new MailAddress(email);

            var smtp = new SmtpClient
            {
                Host = conf["EmailSettings:Host"],
                Port = int.Parse(conf["EmailSettings:Port"]),
                EnableSsl = bool.Parse(conf["EmailSettings:EnableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromMail.Address, conf["EmailSettings:Password"])
            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}
