﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoT.Security.Infra.Data.Policies
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       HasScopeRequirement requirement)
        {
            var scopeClaims = context
                                .User
                                .FindAll(x => x.Type == "scope" && 
                                              x.Issuer == requirement.Issuer)
                                .ToList();
            // If user does not have the scope claim, get out of here
            if (!scopeClaims.Any())
            {
                return Task.CompletedTask;
            }
            // Split the scopes string into an array
            var scopes = scopeClaims.Select(x => x.Value);
            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == requirement.Scope))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}