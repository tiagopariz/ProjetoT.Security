using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/v{version:apiVersion}/public/externallogin/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class ExternalLoginController : ApiBaseController
    {
        public ExternalLoginController(UserManager<User> userManager,
                                       SignInManager<User> signInManager,
                                       IEmailSender emailSender,
                                       ILogger<ExternalLoginController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger)
        {
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            //var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return BadRequest(new { ErrorMessage });
            }
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest(new { ErrorMessage });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                Logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return BadRequest(new { result.IsLockedOut });
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return Ok(new { Email = email, ReturnUrl = returnUrl, info.LoginProvider });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginDtoModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);
                        Logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return Ok(returnUrl);
                    }
                }
                AddErrors(result);
            }
            return BadRequest(model);
        }
    }
}