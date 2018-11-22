using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetoT.Security.Domain.Entities.OAuth;
using ProjetoT.Security.Infra.Data.Context;

namespace ProjetoT.Security.Infra.Data.Providers
{
    public class ProjetoTProvider : OpenIdConnectServerProvider
    {
        // These doesn't exist yet - but they will further down.
        private ValidationService _validationService;
        private TokenService _tokenService;

        public override Task MatchEndpoint(MatchEndpointContext context)
        {
            if (context.Options.AuthorizationEndpointPath.HasValue &&
                context.Request.Path.Value.StartsWith(context.Options.AuthorizationEndpointPath))
            {
                context.MatchAuthorizationEndpoint();
            }
            return Task.CompletedTask;
        }


        #region Authorization Requests

        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            _validationService = context
                                    .HttpContext
                                    .RequestServices
                                        .GetRequiredService<ValidationService>();

            if (!context.Request.IsAuthorizationCodeFlow() && !context.Request.IsImplicitFlow())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: "Only authorization code, refresh token, and token grant types are accepted by this authorization server."
                );
                return;
            }

            var clientId = Guid.Parse(context.ClientId);
            var rdi = context.Request.RedirectUri;
            var scope = context.Request.Scope;

            if (clientId == Guid.Empty)
            {
                context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidClient,
                            description: "client_id cannot be empty"
                        );
                return;
            }
            else if (string.IsNullOrWhiteSpace(rdi))
            {
                context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidClient,
                            description: "redirect_uri cannot be empty"
                        );
                return;
            }
            else if (!await _validationService.CheckClientIdIsValid(clientId))
            {
                context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidClient,
                            description: "The supplied client id does not exist"
                        );
                return;
            }
            else if (!await _validationService.CheckRedirectUriMatchesClientId(clientId, rdi))
            {
                context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidClient,
                            description: "The supplied redirect uri is incorrect"
                        );
                return;
            }
            else if (!_validationService.CheckScopesAreValid(scope))
            {
                context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: "One or all of the supplied scopes are invalid"
                    );
                return;
            }

            context.Validate();

        }

        public override async Task ApplyAuthorizationResponse(ApplyAuthorizationResponseContext context)
        {
            if (!string.IsNullOrWhiteSpace(context.Error))
            {
                return;
            }
            _tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
            var db = context.HttpContext.RequestServices.GetRequiredService<ProjetoTIdentityDbContext>();
            var claimsUser = context.HttpContext.User;
            // Implicit grant is the only flow that gets their token issued here.
            var access = new Token()
            {
                GrantType = OpenIdConnectConstants.GrantTypes.Implicit,
                TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
                Value = context.AccessToken,
            };

            var client = db.ClientApplications.First(x => x.Id == Guid.Parse(context.Request.ClientId));
            if (client == null)
            {
                return;
            }

            if (client.SubordinateTokenLimits == null)
            {
                access.RateLimit = RateLimit.DefaultImplicitLimit;
            }
            else
            {
                access.RateLimit = client.SubordinateTokenLimits;
            }

            await _tokenService.WriteNewTokenToDatabase(Guid.Parse(context.Request.ClientId), access, claimsUser);
        }

        #endregion


        #region Token Requests

        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            Guid clientId;
            string clientSecret;

            _validationService = context.HttpContext.RequestServices.GetRequiredService<ValidationService>();

            // We only accept "authorization_code", "refresh", "token" for this endpoint.
            if (!context.Request.IsAuthorizationCodeGrantType() && 
                !context.Request.IsRefreshTokenGrantType() && 
                !context.Request.IsClientCredentialsGrantType())
            {
                context.Reject(error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                               description: "Only authorization code, refresh token, and token grant types are accepted by this authorization server.");
            }

            // Validating the Authorization Code Token Request
            if (context.Request.IsAuthorizationCodeGrantType())
            {
                clientId = Guid.Parse(context.ClientId);
                clientSecret = context.ClientSecret;

                var redirectUri = context.Request.RedirectUri;

                if (clientId == Guid.Empty)
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "client_id cannot be empty");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "client_secret cannot be empty");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(redirectUri))
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "redirect_uri cannot be empty");
                    return;
                }
                else if (!await _validationService.CheckClientIdIsValid(clientId))
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied client id was does not exist");
                    return;
                }
                else if (!await _validationService.CheckClientIdAndSecretIsValid(clientId, clientSecret))
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied client secret is invalid");
                    return;
                }
                else if (!await _validationService.CheckRedirectUriMatchesClientId(clientId, redirectUri))
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied redirect uri is incorrect");
                    return;
                }

                context.Validate();
            }

            // Validating the Refresh Code Token Request
            else if (context.Request.IsRefreshTokenGrantType())
            {
                clientId = Guid.Parse(context.Request.ClientId);
                clientSecret = context.Request.ClientSecret;
                var refreshtoken = context.Request.RefreshToken;

                if (clientId == Guid.Empty)
                {
                    context.Reject(
                                  error: OpenIdConnectConstants.Errors.InvalidClient,
                              description: "client_id cannot be empty");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.Reject(
                                    error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "client_secret cannot be empty");
                    return;
                }
                else if (!await _validationService.CheckClientIdIsValid(clientId))
                {
                    context.Reject(
                                    error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied client id does not exist");
                    return;
                }
                else if (!await _validationService.CheckClientIdAndSecretIsValid(clientId, clientSecret))
                {
                    context.Reject(
                                    error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied client secret is invalid");
                    return;
                }
                else if (!await _validationService.CheckRefreshTokenIsValid(refreshtoken))
                {
                    context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied refresh token is invalid");
                    return;
                }

                context.Validate();
            }

            // Validating Client Credentials Request, aka, 'token'
            else if (context.Request.IsClientCredentialsGrantType())
            {
                clientId = Guid.Parse(context.ClientId);
                clientSecret = context.ClientSecret;

                if (clientId == Guid.Empty)
                {
                    context.Reject(error: OpenIdConnectConstants.Errors.InvalidClient,
                                   description: "client_id cannot be empty");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.Reject(error: OpenIdConnectConstants.Errors.InvalidClient,
                                   description: "client_secret cannot be empty");
                    return;
                }
                else if (!await _validationService.CheckClientIdIsValid(clientId))
                {
                    context.Reject(
                                    error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied client id does not exist");
                    return;
                }
                else if (!await _validationService.CheckClientIdAndSecretIsValid(clientId, clientSecret))
                {
                    context.Reject(
                                    error: OpenIdConnectConstants.Errors.InvalidClient,
                                description: "The supplied client secret is invalid");
                    return;
                }

                context.Validate();
            }
            else
            {
                context.Reject(
                        error: OpenIdConnectConstants.Errors.ServerError,
                    description: "Could not validate the token request");
            }
        }

        public override Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            AuthenticationTicket ticket;
            // Handling Client Credentials
            if (context.Request.IsClientCredentialsGrantType())
            {
                // If we do not specify any form of Ticket, or ClaimsIdentity, or ClaimsPrincipal, our validation will succeed here but fail later.
                // ASOS needs those to serialize a token, and without any, it fails because there's way to fashion a token properly. Check the ASOS source for more details.
                ticket = TicketCounter.MakeClaimsForClientCredentials(Guid.Parse(context.Request.ClientId));
                context.Validate(ticket);
                return Task.CompletedTask;
            }
            // Handling Authorization Codes
            else if (context.Request.IsAuthorizationCodeGrantType() || context.Request.IsRefreshTokenGrantType())
            {
                ticket = context.Ticket;
                if (ticket != null)
                {
                    context.Validate(ticket);
                    return Task.CompletedTask;
                }
                else
                {
                    context.Reject(
                          error: OpenIdConnectConstants.Errors.InvalidRequest,
                          description: "User isn't valid");
                    return Task.CompletedTask;
                }

            }
            // Catch all error
            context.Reject(
                    error: OpenIdConnectConstants.Errors.ServerError,
                    description: "Could not validate the token request");
            return Task.CompletedTask;
        }

        public override async Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            if (context.Error != null)
            {
                return;
            }
            _tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ProjetoTIdentityDbContext>();
            var client = await dbContext.ClientApplications.FirstOrDefaultAsync(x => x.Id == Guid.Parse(context.Request.ClientId));
            if (client == null)
            {
                return;
            }

            // Implicit Flow Tokens are not returned from the `Token` group of methods - you can find them in the `Authorize` group.
            if (context.Request.IsClientCredentialsGrantType())
            {
                // The only thing returned from a successful client grant is a single `Token`
                var t = new Token()
                {
                    TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
                    GrantType = OpenIdConnectConstants.GrantTypes.ClientCredentials,
                    Value = context.Response.AccessToken,
                };

                await _tokenService.WriteNewTokenToDatabase(Guid.Parse(context.Request.ClientId), t);
            }
            else if (context.Request.IsAuthorizationCodeGrantType())
            {
                var access = new Token()
                {
                    TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
                    GrantType = OpenIdConnectConstants.GrantTypes.AuthorizationCode,
                    Value = context.Response.AccessToken,
                };
                var refresh = new Token()
                {
                    TokenType = OpenIdConnectConstants.TokenUsages.RefreshToken,
                    GrantType = OpenIdConnectConstants.GrantTypes.AuthorizationCode,
                    Value = context.Response.RefreshToken,
                };

                await _tokenService.WriteNewTokenToDatabase(Guid.Parse(context.Request.ClientId), access, context.Ticket.Principal);
                await _tokenService.WriteNewTokenToDatabase(Guid.Parse(context.Request.ClientId), refresh, context.Ticket.Principal);
            }
            else if (context.Request.IsRefreshTokenGrantType())
            {
                var access = new Token()
                {
                    TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
                    GrantType = OpenIdConnectConstants.GrantTypes.AuthorizationCode,
                    Value = context.Response.AccessToken,
                };
                await _tokenService.WriteNewTokenToDatabase(Guid.Parse(context.Request.ClientId), access, context.Ticket.Principal);
            }
        }

        #endregion
    }
}