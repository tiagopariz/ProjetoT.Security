using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Context;
using ProjetoT.Security.Infra.Messages.Email.Interfaces;
using ProjetoT.Security.Services.WebApi.DtoModels.OAuth;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    [ApiVersion(CurrentApiVersion)]
    [Route("api/v{version:apiVersion}/public/clientapplications/")]
    [ApiController]
    [EnableCors("AllowLocalhostOrigin")]
    public class ClientApplicationController : ApiBaseController
    {
        public ClientApplicationController(UserManager<User> userManager,
                                                SignInManager<User> signInManager,
                                                IEmailSender emailSender,
                                                ILogger<ClientApplicationController> logger,
                                                ProjetoTIdentityDbContext context)
            : base(userManager,
                   signInManager,
                   emailSender,
                   logger,
                   context: context)
        { }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            var uid = Guid.Parse(UserManager.GetUserId(User));
            return Ok(await Context.ClientApplications
                                    .Include(x => x.Owner)
                                    .Where(x => x.Owner.Id == uid)
                                    .ToListAsync());
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post(ClientApplicationDtoModel clientApplicationDtoModel)
        {
            if (!ModelState.IsValid || !User.Identity.IsAuthenticated)
                return BadRequest("Dados inválidos ou usuário não autenticado.");

            var owner = await UserManager.GetUserAsync(User);
            var clientApplication = new ClientApplication
            {
                ClientDescription = clientApplicationDtoModel.ClientDescription,
                ClientName = clientApplicationDtoModel.ClientName,
                Id = Guid.NewGuid(),
                ClientSecret = Guid.NewGuid().ToString(),
                Owner = owner,
                RateLimit = RateLimit.DefaultClientLimit
            };

            Context.Add(clientApplication);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Put(ClientApplicationDtoModel clientApplicationDtoModel)
        {
            var uid = Guid.Parse(UserManager.GetUserId(this.User));
            var clientApplication = await Context.ClientApplications
                                            .Include(x => x.Owner)
                                            .Include(x => x.RedirectUris)
                                            .Where(x => x.Id == clientApplicationDtoModel.Id &&
                                                        x.Owner.Id == uid)
                                            .FirstOrDefaultAsync();
            if (clientApplication == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalUris = clientApplication.RedirectUris;
                    CheckAndMark(originalUris, clientApplicationDtoModel.RedirectUris);

                    clientApplication.ClientDescription = clientApplicationDtoModel.ClientDescription;
                    Context.Update(clientApplication);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(clientApplicationDtoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return Ok(clientApplicationDtoModel);

            void CheckAndMark(List<RedirectUri> originals, IEnumerable<string> submitted)
            {
                var newList = new List<RedirectUri>();
                foreach (var s in submitted)
                {
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        continue;
                    }
                    var fromOld = originals.FirstOrDefault(x => x.URI == s);
                    if (fromOld == null)
                    {
                        // this 's' is new.
                        var rdi = new RedirectUri()
                        {
                            ClientApplication = clientApplication,
                            ClientApplicationId = clientApplication.Id,
                            URI = s
                        };
                        newList.Add(rdi);
                    }
                    else
                    {
                        // this 's' was re-submitted
                        newList.Add(fromOld);
                    }
                }

                // Marking deleted Redirect URIs for Deletion.
                originals.Except(newList).Select(x => Context.Entry(x).State = EntityState.Deleted);

                // Assign the new list back to the client
                clientApplication.RedirectUris = newList;
            }
        }

        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(Guid id)
        {

            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var uid = Guid.Parse(UserManager.GetUserId(this.User));
            var clientApplication = await Context.ClientApplications
                                        .Include(x => x.Owner)
                                        .SingleOrDefaultAsync(m => m.Id == id && m.Owner.Id == uid);

            if (clientApplication == null)
            {
                return NotFound();
            }

            Context.ClientApplications.Remove(clientApplication);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Route("ResetSecret")]
        public async Task<IActionResult> ResetSecret(Guid id)
        {
            var uid = Guid.Parse(UserManager.GetUserId(User));
            var client = await Context.ClientApplications
                                            .Include(x => x.Owner)
                                            .Include(x => x.RedirectUris)
                                            .Where(x => x.Id == id && x.Owner.Id == uid)
                                            .FirstOrDefaultAsync();
            if (client == null)
            {
                return NotFound();
            }

            try
            {
                client.ClientSecret = Guid.NewGuid().ToString();
                Context.Update(client);
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(client.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { url = "clientapplications/Edit/" + id });
        }

        private bool ClientExists(Guid id)
        {
            return Context.ClientApplications.Any(x => x.Id == id);
        }
    }
}