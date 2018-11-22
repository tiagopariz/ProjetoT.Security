using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/lockout/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class LockoutController : ApiBaseController
    {
        public LockoutController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IEmailSender emailSender,
                                 ILogger<LockoutController> logger)
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
        public IActionResult Lockout()
        {
            return Ok(new { ReturnLoginUrl = "ReturnLoginUrl" });
        }
    }
}