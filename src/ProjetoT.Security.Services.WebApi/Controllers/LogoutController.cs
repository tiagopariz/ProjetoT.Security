using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/logout/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class LogoutController : ApiBaseController
    {
        public LogoutController(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                IEmailSender emailSender,
                                ILogger<LogoutController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger)
        {
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            Logger.LogInformation("User logged out.");
            return Ok(new
            {
                ReturnLoginUrl = "returnLoginUrl"
            });
        }
    }
}