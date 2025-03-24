using Microsoft.Extensions.Options;
using PFA_TEMPLATE.Interfaces;
using PFA_TEMPLATE.ViewModels;
using System.Net.Mail;
using System.Net;
namespace PFA_TEMPLATE.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName);
            var toAddress = new MailAddress(toEmail);
            using (var message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true; // Set to true if you want to send HTML emails

                using (var smtp = new SmtpClient
                {
                    Host = _emailSettings.SmtpServer,
                    Port = _emailSettings.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
                })
                {
                    try
                    {
                        await smtp.SendMailAsync(message);
                        Console.WriteLine("Email sent successfully.");
                    }
                    catch (SmtpException smtpEx)
                    {
                        Console.WriteLine($"SMTP Error: {smtpEx.StatusCode}");
                        Console.WriteLine($"SMTP Message: {smtpEx.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                        throw;
                    }
                }
            }
        }
    }
}