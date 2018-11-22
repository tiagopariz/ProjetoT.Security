using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/home/")]
    [ApiController]
    public class HomeController : ApiBaseController
    {
        public HomeController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IEmailSender emailSender,
                              ILogger<LockoutController> logger)
            : base(userManager,
                signInManager,
                emailSender,
                logger)
        {
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Index()
        {
            return Ok(new { Online = "True" });
        }
    }
}