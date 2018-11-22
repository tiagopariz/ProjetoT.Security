using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Services.WebApi.DtoModels;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/loginwithrecoverycode/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class LoginWithRecoveryCodeController : ApiBaseController
    {
        public LoginWithRecoveryCodeController(UserManager<User> userManager,
                                               SignInManager<User> signInManager,
                                               IEmailSender emailSender,
                                               ILogger<LoginWithRecoveryCodeController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger)
        {
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("")]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            return Ok(new { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Route("")]
        public async Task<IActionResult> Login(LoginWithRecoveryCodeDtoModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Ok(model);
            }

            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return Ok(new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                Logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return BadRequest(new { Lockout = "lockoutAction" });
            }
            else
            {
                Logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return BadRequest();
            }
        }
    }
}