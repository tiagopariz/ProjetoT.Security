using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/confirmemail/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class ConfirmEmailController : ApiBaseController
    {
        public ConfirmEmailController(UserManager<User> userManager,
                                      SignInManager<User> signInManager,
                                      IEmailSender emailSender,
                                      ILogger<ConfirmEmailController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (userId == Guid.Empty || code == null)
            {
                return BadRequest(new {ErrorMessage = "User is null"});
            }
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await UserManager.ConfirmEmailAsync(user, code);
            return Ok(new { Result = result.Succeeded ? "ConfirmEmail" : "Error"});
        }
    }
}