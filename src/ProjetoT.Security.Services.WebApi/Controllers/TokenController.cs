using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Infra.Messages.Email.Interfaces;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/token/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class TokenController : ApiBaseController
    {
        public TokenController(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailSender emailSender,
                               ILogger<LoginController> logger)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger) { }

        [HttpGet]
        [Produces("application/json")]
        [Authorize("Bearer")]
        [Route("TestBearer")]
        public IActionResult Test(string returnUrl = null)
        {
            return Ok(new
            {
                TestBearer = "Ok"
            });
        }
    }
}