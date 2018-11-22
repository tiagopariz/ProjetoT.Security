using System.Threading.Tasks;
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
    [Route("api/v{version:apiVersion}/public/register/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class RegisterController : ApiBaseController
    {
        public RegisterController(UserManager<User> userManager,
                                  SignInManager<User> signInManager,
                                  IEmailSender emailSender,
                                  ILogger<RegisterController> logger)
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
        public IActionResult Register(string returnUrl = null)
        {
            return Ok(new { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register(RegisterDtoModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Logger.LogInformation("User created a new account with password.");

                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await EmailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await SignInManager.SignInAsync(user, isPersistent: false);
                    Logger.LogInformation("User created a new account with password.");
                    return Ok(new { ReturnUrl = returnUrl });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(model);
        }


    }
}