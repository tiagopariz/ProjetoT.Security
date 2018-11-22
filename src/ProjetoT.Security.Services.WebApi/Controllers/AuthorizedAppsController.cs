using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Context;
using ProjetoT.Security.Infra.Messages.Email.Interfaces;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/test/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class AuthorizedAppsController : ApiBaseController
    {
        public AuthorizedAppsController(UserManager<User> userManager,
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
        public async Task<IActionResult> AuthorizedApps()
        {
            var uid = UserManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(uid))
            {
                throw new ApplicationException($"Unable to load user with ID '{uid}'.");
            }

            IEnumerable<Token> userstokens = (await Context
                                                        .Users
                                                            .Include(x => x.UserTokens)
                                                            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(uid)))?
                                                                .UserTokens;
            if (userstokens == null)
            {
                throw new ApplicationException($"Unable to load user apps for user ID '{uid}'.");
            }

            IList<ClientApplication> items = Context
                                                .ClientApplications
                                                    .Include(x => x.UserTokens)
                                                    .Where(x => x.UserTokens.Any(y => userstokens.Contains(y)))
                                                    .ToList();
            var authorizedApps = new AuthorizedApps()
            {
                Apps = items,
            };
            return Ok(authorizedApps);
        }

        [HttpPost, ActionName("revoke/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(Guid id)
        {
            var uid = UserManager.GetUserId(User);
            var user = await Context
                                .Users
                                    .Include(x => x.UserTokens)
                                    .FirstOrDefaultAsync(x => x.Id == Guid.Parse(uid));
            if (user == null || string.IsNullOrWhiteSpace(uid))
            {
                throw new ApplicationException($"Unable to load user with ID '{uid}'.");
            }
            var client = await Context
                                .ClientApplications
                                    .Include(x => x.UserTokens)
                                    .FirstOrDefaultAsync(x => x.Id == id);
            if (id == Guid.Empty || client == null)
            {
                throw new ApplicationException("Supplied client id was invalid");
            }

            var tokens = client
                            .UserTokens
                                .Intersect(client.UserTokens)
                                .ToList();
            foreach (var t in tokens)
            {
                Context.Tokens.Remove(t);
                client.UserTokens.Remove(t);
                user.UserTokens.Remove(t);
            }
            Context.ClientApplications.Update(client);
            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok();
        }
    }
}
