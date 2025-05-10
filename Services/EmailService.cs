
using System.Net;
using System.Net.Mail;

namespace Job_Portal_Project.Services
{
    public class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                // Get SMTP settings from configuration
                var host = Configuration["Email:Smtp:Host"];
                var port = int.Parse(Configuration["Email:Smtp:Port"]);
                var username = Configuration["Email:Smtp:Username"];
                var password = Configuration["Email:Smtp:Password"];
                var enableSsl = bool.Parse(Configuration["Email:Smtp:EnableSsl"]);
                client.Host = host;
                client.Port = port;
                client.EnableSsl = enableSsl;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);

                // Create the email message

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.From = new MailAddress(Configuration["Email:Smtp:From"]);
                    emailMessage.To.Add(email);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(emailMessage);
                }
            }


        }
    }
}
