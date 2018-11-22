using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Infra.Data.Context;
using ProjetoT.Security.Infra.Messages.Email.Interfaces;
using ProjetoT.Security.Services.WebApi.Attributes;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/test/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class TestController : ApiBaseController
    {
        public TestController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IEmailSender emailSender,
                              ILogger<LoginController> logger,
                              ProjetoTIdentityDbContext context)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger, 
                   context: context) { }

        [HttpGet]
        [Produces("application/json")]
        [Route("hello")]
        public IActionResult Hello()
        {
            return Ok("Hello");
        }

        // Authenticated Methods - only available to those with a valid Access Token
        // Unscoped Methods - Authenticated methods that do not require any specific Scope
        [HttpGet]
        [Produces("application/json")]
        [Route("clientcount")]
        [RateLimit]
        [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ClientCount()
        {
            return Ok("Client Count Get Request was successful but this endpoint is not yet implemented");
        }

        // Scoped Methods - Authenticated methods that require certain scopes
        [HttpGet]
        [Produces("application/json")]
        [Route("birthdate")]
        [RateLimit] 
        [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-read-birthdate")]
        public IActionResult GetBirthdate()
        {
            return Ok("Birthdate Get Request was successful but this endpoint is not yet implemented");
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("email")]
        [RateLimit]
        [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-read-email")]
        public async Task<IActionResult> GetEmail()
        {
            return Ok("Email Get Request was successful but this endpoint is not yet implemented");
        }

        [HttpPut]
        [Produces("application/json")]
        [Route("birthdate")]
        [RateLimit]
        [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-modify-birthdate")]
        public IActionResult ChangeBirthdate(string birthdate)
        {
            return Ok("Birthdate Put successful but this endpoint is not yet implemented");
        }

        [HttpPut]
        [Produces("application/json")]
        [Route("email")]
        [RateLimit]
        [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-modify-email")]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            return Ok("Email Put request received, but function is not yet implemented");
        }

        // Dynamic Scope Methods - Authenticated methods that return additional information the more scopes are supplied
        [HttpGet]
        [Produces("application/json")]
        [Route("me")]
        [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Me()
        {
            return Ok("User Profile Get request received, but function is not yet implemented");
        }
    }
}
