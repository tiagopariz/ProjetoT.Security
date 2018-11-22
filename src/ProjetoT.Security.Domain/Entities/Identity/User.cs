using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProjetoT.Security.Domain.Entities.OAuth;

namespace ProjetoT.Security.Domain.Entities.Identity
{
    public class User : IdentityUser<Guid>
    {
        /* The list of tokens that have been issued for a given user, across all applications */
        public List<Token> UserTokens { get; set; } = new List<Token>();
        /* The list of client applications a user has created. This is not the same as the UserClientTokens list. */
        public List<ClientApplication> ClientsApplications { get; set; } = new List<ClientApplication>();
    }   
}