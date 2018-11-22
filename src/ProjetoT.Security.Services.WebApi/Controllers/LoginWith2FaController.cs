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
    [Route("api/v{version:apiVersion}/public/loginwith2fa/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class LoginWith2FaController : ApiBaseController
    {
        public LoginWith2FaController(UserManager<User> userManager,
                                      SignInManager<User> signInManager,
                                      IEmailSender emailSender,
                                      ILogger<LoginWith2FaController> logger)
            : base(userManager,
                signInManager,
                emailSender,
                logger) { }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("")]
        public async Task<IActionResult> Login(bool rememberMe,
                                                      string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2FaDtoModel
            {
                RememberMe = rememberMe,
                ReturnUrl = returnUrl
            };
            return Ok(model);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Route("")]
        public async Task<IActionResult> Login(LoginWith2FaDtoModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return Ok(new { ReturnUrl = returnUrl });
            }
            else if (result.IsLockedOut)
            {
                Logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return BadRequest(new { Lockout = "lockoutAction" });
            }
            else
            {
                Logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return BadRequest(model);
            }
        }
    }
}