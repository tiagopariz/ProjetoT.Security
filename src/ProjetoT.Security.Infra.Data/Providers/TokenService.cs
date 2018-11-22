using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Context;

namespace ProjetoT.Security.Infra.Data.Providers
{
    public class TokenService
    {
        private readonly ProjetoTIdentityDbContext _context;
        private readonly UserManager<User> _userManager;

        public TokenService(ProjetoTIdentityDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task WriteNewTokenToDatabase(Guid clientId, 
                                                  Token token, 
                                                  ClaimsPrincipal user = null)
        {
            if (clientId == Guid.Empty || 
                token == null || 
                string.IsNullOrWhiteSpace(token.GrantType) || 
                string.IsNullOrWhiteSpace(token.Value))
            {
                return;
            }

            var client = await _context
                                    .ClientApplications
                                        .Include(x => x.Owner)
                                        .Include(x => x.UserTokens)
                                        .Where(x => x.Id == clientId)
                                        .FirstOrDefaultAsync();
            if (client == null)
            {
                return;
            }

            // Handling Client Creds
            if (token.GrantType == OpenIdConnectConstants.GrantTypes.ClientCredentials)
            {
                var oldClientCredentialTokens = client
                                                    .UserTokens
                                                        .Where(x => x.GrantType == OpenIdConnectConstants.GrantTypes.ClientCredentials)
                                                        .ToList();
                foreach (var old in oldClientCredentialTokens)
                {
                    _context.Entry(old).State = EntityState.Deleted;
                    client.UserTokens.Remove(old);
                }
                client.UserTokens.Add(token);
                _context.Update(client);
                await _context.SaveChangesAsync();
            }
            // Handling the other flows
            else if (token.GrantType == OpenIdConnectConstants.GrantTypes.Implicit || 
                     token.GrantType == OpenIdConnectConstants.GrantTypes.AuthorizationCode || 
                     token.GrantType == OpenIdConnectConstants.GrantTypes.RefreshToken)
            {
                if (user == null)
                {
                    return;
                }
                var au = await _userManager.GetUserAsync(user);
                if (au == null)
                {
                    return;
                }

                // These tokens also require association to a specific user
                IEnumerable<Token> oldTokensForGrantType = client
                                                                .UserTokens
                                                                    .Where(x => x.GrantType == token.GrantType && 
                                                                                x.TokenType == token.TokenType)
                                                                    .Intersect(au.UserTokens).ToList();
                foreach (var old in oldTokensForGrantType)
                {
                    _context.Entry(old).State = EntityState.Deleted;
                    client.UserTokens.Remove(old);
                    au.UserTokens.Remove(old);
                }
                client.UserTokens.Add(token);
                au.UserTokens.Add(token);
                _context.ClientApplications.Update(client);
                _context.Users.Update(au);
                await _context.SaveChangesAsync();
            }
        }
    }
}