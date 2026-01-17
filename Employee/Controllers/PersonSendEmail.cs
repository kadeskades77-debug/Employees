using EMPLOYEE.Service;
using EMPLOYEE.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMPLOYEE.Controllers
{
    [Route("api/[controller]")]
    public class PersonSendEmail : ControllerBase
    {
        private readonly IPersonalEmailSend _sendEmail;

        public PersonSendEmail(IPersonalEmailSend sendEmail)
        {
            _sendEmail = sendEmail; 
        }
        [HttpGet("throw")]
        public IActionResult ThrowException()
        {
            throw new Exception("Test Slack Middleware Exception!");
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddAsync(EmailSettings model)
        {
            var Uemail = User?.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(Uemail))
                return Unauthorized("Email not found in token.");

            var result = await _sendEmail.AddAsync(model, Uemail);

            return Ok(result);
        }
        [Authorize]
        [HttpPost("Send_email")]
        public async Task<IActionResult> SendAsync(string to, string subject, string Body)
        {
            var Uemail = User?.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(Uemail))
                return Unauthorized("Email not found in token.");

            var result = await _sendEmail.SendWithMyEmailAsync(Uemail,to, subject,Body);

            return Ok(result);
        }
    }
}
