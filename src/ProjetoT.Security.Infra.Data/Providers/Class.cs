using System;
using System.Collections.Generic;
using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;

namespace ProjetoT.Security.Infra.Data.Providers
{
    public static class TicketCounter
    {
        public static AuthenticationTicket MakeClaimsForClientCredentials(Guid clientId)
        {
            var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, 
                                              OpenIdConnectConstants.Claims.Name,
                                              OpenIdConnectConstants.Claims.Role);

            identity.AddClaim(
                new Claim(OpenIdConnectConstants.Claims.Subject,
                          clientId.ToString())
                            .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));


            // We serialize the grant_type so we can user discriminate rate-limits. AuthorizationCode grants typically have the highest rate-limit allowance
            identity.AddClaim(
                new Claim("grant_type", OpenIdConnectConstants.GrantTypes.ClientCredentials)
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));

            // We serialize the client_id so we can monitor for usage patterns of a given app, and also to allow for app-based token revokes.
            identity.AddClaim(
                new Claim("client_id", clientId.ToString())
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));


            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), 
                                                  new AuthenticationProperties(), 
                                                  OpenIdConnectServerDefaults.AuthenticationScheme);

            // In our implementation, an access token is valid for a single hour.
            return ticket;
        }

        public static AuthenticationTicket MakeClaimsForInteractive(User user, Authorize authorize)
        {
            /*
                *  If you want to issue an OpenId Token, the spec for which is available at https://openid.net/connect/
                *  Then in each of the SetDestinations, add a reference to OpenIdConnect.Destinations.IdentityToken, like so:
                *  
                *  new Claim("grant_type", OpenIdConnectConstants.GrantTypes.AuthorizationCode)
                *         .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));
                *         
                *   This ensures that the claims you are concerned about will be placed into the Identity Token, which other services may access.
                */
            var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, 
                                              OpenIdConnectConstants.Claims.Name,
                                              OpenIdConnectConstants.Claims.Role);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()).SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.NormalizedUserName).SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));
            identity.AddClaim(new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp).SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));

            // We serialize the user_id so we can determine which user the caller of this token is
            identity.AddClaim(
                    new Claim(OpenIdConnectConstants.Claims.Subject, user.Id.ToString())
                        .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));

            switch (authorize.ResponseType)
            {
                // We serialize the grant_type so we can user discriminate rate-limits. AuthorizationCode grants typically have the highest rate-limit allowance
                case OpenIdConnectConstants.ResponseTypes.Code:
                    identity.AddClaim(
                        new Claim("grant_type", OpenIdConnectConstants.GrantTypes.AuthorizationCode)
                            .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));
                    break;
                case OpenIdConnectConstants.ResponseTypes.Token:
                    identity.AddClaim(
                        new Claim("grant_type", OpenIdConnectConstants.GrantTypes.Implicit)
                            .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));
                    break;
            }

            // We serialize the client_id so we can monitor for usage patterns of a given app, and also to allow for app-based token revokes.
            identity.AddClaim(
                    new Claim("client_id", authorize.ClientId.ToString())
                        .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken));


            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity),
                                                  new AuthenticationProperties(),
                                                  OpenIdConnectServerDefaults.AuthenticationScheme);

            ICollection<string> scopesToAdd = new List<string>()
            {
                /* If  you've chosen to add an OpenId token to your destinations, be sure to include the OpenIdCOnnectConstants.Scopes.OpenId in this list */
                //OpenIdConnectConstants.Scopes.OpenId, // Lets our requesting clients know that an OpenId Token was generated with the original request.
            };

            if (authorize.ResponseType == OpenIdConnectConstants.ResponseTypes.Code)
            {
                scopesToAdd.Add(OpenIdConnectConstants.Scopes.OfflineAccess); //Gives us a RefreshToken, only do this if we're following the `Authorization Code` flow. For `Implicit Grant`, we don't supply a refresh token.    
            }
            foreach (var s in authorize.Scopes)
            {
                if (Scope.NameInScopes(s))
                {
                    scopesToAdd.Add(s);
                }
            }

            ticket.SetScopes(scopesToAdd);

            return ticket;
        }
    }
}
