using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;


namespace Forum.Services
{
    public class MailConfirmationSender
    {
        private IConfiguration conf;
        private IMailSender sender;
        public MailConfirmationSender(IConfiguration conf, IMailSender sender)
        {
            this.conf = conf;
            this.sender = sender;
        }
        public void Send(string username, string email, string id, string activationcode)
        {
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(activationcode);
            activationcode = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            string host = conf["EmailSettings:LinkHost"];
            var verifyUrl = $"{host}/Account/Verify?id={id}&token={activationcode}";
            string subject = "Verify your account";
            string body = $"<br/><br/>We are excited to tell you, <strong>{username}</strong>, that your account is" +
         " successfully created. Please click on the below link to verify your account" +
         " <br/><br/><a href='" + verifyUrl + "'>" + verifyUrl + "</a> ";
            sender.SendMail(email, body, subject);
        }
    }
}
