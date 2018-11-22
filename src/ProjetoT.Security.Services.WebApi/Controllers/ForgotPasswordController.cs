using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Services.WebApi.DtoModels;
using ProjetoT.Security.Services.WebApi.Extensions;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/forgotpassword/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class ForgotPasswordController : ApiBaseController
    {
        public ForgotPasswordController(UserManager<User> userManager,
                                        SignInManager<User> signInManager,
                                        IEmailSender emailSender,
                                        ILogger<ForgotPasswordController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDtoModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user)))
                {
                    return BadRequest(new { ErrorMessage = @"Don't reveal that the user does not exist or is not confirmed" });
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await EmailSender.SendEmailAsync(model.Email, "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return Ok(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return Ok();
        }
    }
}