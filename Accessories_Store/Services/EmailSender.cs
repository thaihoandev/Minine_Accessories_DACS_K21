using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace Accessories_Store.Services
{

    public class MyEmailSender : IMyEmailSender
    {
        public IConfiguration Configuration { get; }
        public MyEmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void SendEmail(string email, string subject, string HtmlMessage)
        {
            using (MailMessage mm = new MailMessage(Configuration["NetMail:sender"], email))
            {
                mm.Subject = subject;
                string body = HtmlMessage;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Configuration["NetMail:smtpHost"];
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(Configuration["NetMail:sender"], Configuration["NetMail:senderpassword"]);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
    }
}
