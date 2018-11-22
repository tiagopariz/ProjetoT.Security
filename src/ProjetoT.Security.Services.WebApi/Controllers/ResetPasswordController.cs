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
    [Route("api/v{version:apiVersion}/public/resetpassword/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class ResetPasswordController : ApiBaseController
    {
        public ResetPasswordController(UserManager<User> userManager,
                                       SignInManager<User> signInManager,
                                       IEmailSender emailSender,
                                       ILogger<ResetPasswordController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger) { }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest(new
                {
                    ErrorMessage = "A code must be supplied for password reset."
                });
            }
            var model = new ResetPasswordDtoModel { Code = code };
            return Ok(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDtoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { ErrorMessage = @"Don't reveal that the user does not exist" });
            }
            var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok(model);
            }
            AddErrors(result);
            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return Ok();
        }
    }
}