using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/accessdenied/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class AccessDeniedController : ApiBaseController
    {
        public AccessDeniedController(UserManager<User> userManager,
                                      SignInManager<User> signInManager,
                                      IEmailSender emailSender,
                                      ILogger<AccessDeniedController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger) { }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Ok(new { RedirectUrl = "" });
        }
    }
}