using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; 
namespace MS_Contact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService; 
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService,  ILogger<EmailController> logger)
        {
            _emailService = emailService; 
            _logger = logger;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(
           [FromQuery] string recipientName,
           [FromQuery] string recipientEmail,
           [FromQuery] string subject,
           [FromQuery] string body)
        {
            var result = await _emailService.SendEmailAsync(recipientName, recipientEmail, subject, body);

            if (result)
            {
                return Ok("Email sent successfully.");
            }

            return StatusCode(500, "Failed to send email.");
        }

        
    }
}
