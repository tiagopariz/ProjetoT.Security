using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.EntityFrameworkCore;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Context;

namespace ProjetoT.Security.Infra.Data.Providers
{
    public class ValidationService
    {

        private readonly ProjetoTIdentityDbContext _context;

        public ValidationService(ProjetoTIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckClientIdIsValid(Guid clientId)
        {
            if (clientId == Guid.Empty)
                return false;

            return await _context
                            .ClientApplications
                                .AnyAsync(x => x.Id == clientId);
        }

        public async Task<bool> CheckClientIdAndSecretIsValid(Guid clientId,
                                                              string clientSecret)
        {
            if (clientId == Guid.Empty || 
                string.IsNullOrWhiteSpace(clientSecret))
            {
                return false;
            }

            // This could be an easy check, but the ASOS maintainer strongly recommends you to use a fixed-time string compare for client secrets.
            // This is trivially available in any .NET Core 2.1 or higher framework, but this is a 2.0 project, so we will leave that part out.
            // If you are on 2.1+, checkout the System.Security.Cryptography.CryptographicOperations.FixedTimeEquals() mehod,
            // available at https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals?view=netcore-2.1
            return await _context
                            .ClientApplications
                                .AnyAsync(x => x.Id == clientId && 
                                               x.ClientSecret == clientSecret);
        }

        public async Task<bool> CheckRedirectUriMatchesClientId(Guid clientId,
                                                                string redirectUri)
        {
            if (clientId == Guid.Empty || 
                string.IsNullOrWhiteSpace(redirectUri))
            {
                return false;
            }
            return await _context
                            .ClientApplications
                                .Include(x => x.RedirectUris)
                                .AnyAsync(x => x.Id == clientId &&
                                               x.RedirectUris.Any(y => y.URI == redirectUri));
        }

        public async Task<bool> CheckRefreshTokenIsValid(string refresh)
        {
            if (string.IsNullOrWhiteSpace(refresh))
            {
                return false;
            }

            return await _context
                            .ClientApplications
                                .Include(x => x.UserTokens)
                                .AnyAsync(x => x.UserTokens
                                                    .Any(y => y.TokenType == OpenIdConnectConstants.TokenUsages.RefreshToken && 
                                                              y.Value == refresh));
        }

        public bool CheckScopesAreValid(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                return true; // Unlike the other checks, an empty scope is a valid scope. It just means the application has default permissions.
            }

            var scopes = scope.Split(' ');
            foreach (var s in scopes)
            {
                if (!Scope.NameInScopes(s))
                {
                    return false;
                }
            }
            return true;
        }
    }
}