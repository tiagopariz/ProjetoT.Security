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
    [Route("api/v{version:apiVersion}/public/login/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class LoginController : ApiBaseController
    {
        public LoginController(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailSender emailSender,
                               ILogger<LoginController> logger)
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
        public IActionResult Login(string returnUrl = null)
        {
            return Ok(new
            {
                ReturnUrl = returnUrl,
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated
            });
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login(LoginDtoModel model,
                                               string returnUrl = null)
        {
            string log;

            if (!ModelState.IsValid)
            {
                log = "User error login.";
                Logger.LogWarning(log);
                return StatusCode(500, new
                {
                    ReturnUrl = returnUrl,
                    RememberMe = model.RememberMe,
                    LogInformation = log
                });
            }

            var result = await SignInManager
                                .PasswordSignInAsync(model.Email,
                                                     model.Password,
                                                     model.RememberMe,
                                                     true);
            if (result.Succeeded)
            {
                log = "User logged in.";
                Logger.LogInformation(log);


                return Ok(new
                {
                    ReturnUrl = returnUrl,
                    Result = result,
                    LogInformation = log,
                    Authenticated = true,
                    Message = "OK"
                });
            }

            if (result.RequiresTwoFactor)
            {
                log = "User Requires two factor login.";
                Logger.LogWarning(log);
                return BadRequest(new
                {
                    ReturnUrl = returnUrl,
                    RememberMe = model.RememberMe,
                    LoginUrl = "Login/With2Fa",
                    LogInformation = log
                });
            }

            if (result.IsLockedOut)
            {
                log = "User account locked out.";
                Logger.LogWarning(log);
                return BadRequest(new
                {
                    ReturnUrl = returnUrl,
                    RememberMe = model.RememberMe,
                    LogInformation = log
                });
            }

            log = "Invalid login attempt.";
            Logger.LogWarning(log);
            return BadRequest(new
            {
                ReturnUrl = returnUrl,
                RememberMe = model.RememberMe,
                LogInformation = log
            });
        }
    }
}