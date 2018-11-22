using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoT.Security.Infra.Data.Policies
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; set; }
        public string Scope { get; set; }

        public HasScopeRequirement(string scope,
                                   string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }
}