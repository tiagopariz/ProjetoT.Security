using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Context;
using ProjetoT.Security.Infra.Data.Providers;

namespace ProjetoT.Security.Services.WebApi.Controllers
{
    public class AuthorizeController : Controller
    {
        private readonly ProjetoTIdentityDbContext _context;
        private readonly UserManager<User> _userManager;

        public AuthorizeController(ProjetoTIdentityDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var request = HttpContext.GetOpenIdConnectRequest();
            var client = _context
                                            .ClientApplications
                                                .FirstOrDefault(x => x.Id == Guid.Parse(request.ClientId));
            if (client == null)
            {
                return NotFound();
            }

            var authorize = new Authorize()
            {
                ClientId = client.Id,
                ClientDescription = client.ClientDescription,
                ClientName = client.ClientName,
                RedirectUri = request.RedirectUri,
                ResponseType = request.ResponseType,
                Scopes = string.IsNullOrWhiteSpace(request.Scope) ? new string[0] : request.Scope.Split(' '),
                State = request.State
            };
            return Ok(authorize);
        }

        [HttpPost("deny")]
        public async Task<IActionResult> Deny()
        {
            return LocalRedirect("/");
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Accept()
        {
            var au = await _userManager.GetUserAsync(HttpContext.User);
            if (au == null)
            {
                return BadRequest("Usuário nulo");
            }
            var request = HttpContext.GetOpenIdConnectRequest();
            var authorize = await FillFromRequest(request);
            if (authorize == null)
            {
                return BadRequest("Sem autorização");
            }
            var ticket = TicketCounter.MakeClaimsForInteractive(au, authorize);
            var signInResult = SignIn(ticket.Principal, 
                                      ticket.Properties,
                                      ticket.AuthenticationScheme);
            return signInResult;
        }

        private async Task<Authorize> FillFromRequest(OpenIdConnectRequest OIDCRequest)
        {
            var clientId = OIDCRequest.ClientId;
            var client = await _context.ClientApplications.FindAsync(clientId);
            if (client == null)
            {
                return null;
            }
            else
            {
                // Get the Scopes for this application from the query - disallow duplicates
                var scopes = new HashSet<Scope>();
                if (!string.IsNullOrWhiteSpace(OIDCRequest.Scope))
                {
                    foreach (var s in OIDCRequest.Scope.Split(' '))
                    {
                        if (Scope.NameInScopes(s))
                        {
                            var scope = Scope.GetScope(s);
                            if (!scopes.Contains(scope))
                            {
                                scopes.Add(scope);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                var authorize = new Authorize()
                {
                    ClientId = Guid.Parse(OIDCRequest.ClientId),
                    ResponseType = OIDCRequest.ResponseType,
                    State = OIDCRequest.State,
                    Scopes = string.IsNullOrWhiteSpace(OIDCRequest.Scope) ? new string[0] : OIDCRequest.Scope.Split(' '),
                    RedirectUri = OIDCRequest.RedirectUri
                };

                return authorize;
            }
        }
    }
}