using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Infra.Data.Context;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        public readonly UserManager<User> UserManager;
        public readonly SignInManager<User> SignInManager;
        public readonly ProjetoTIdentityDbContext Context;
        public readonly IEmailSender EmailSender;
        public readonly ILogger Logger;
        public const string CurrentApiVersion = "0.1";

        public ApiBaseController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IEmailSender emailSender,
                                 ILogger<ApiBaseController> logger,
                                 ProjetoTIdentityDbContext context = null)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
            Logger = logger;
            Context = context;
        }


        [TempData]
        public string ErrorMessage { get; set; }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #region Helpers

        protected IActionResult RedirectToLocal(string returnUrl)
        {
            return Ok(new
            {
                ReturnUrl = returnUrl,
                IsLocalUrl = Url.IsLocalUrl(returnUrl)
            });
        }

        #endregion
    }
}