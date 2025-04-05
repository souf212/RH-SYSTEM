using PFA_TEMPLATE.ViewModels;

 
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string recipientName, string recipientEmail, string subject, string body);
    }

