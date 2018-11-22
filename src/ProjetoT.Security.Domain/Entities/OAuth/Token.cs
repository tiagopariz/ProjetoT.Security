using System;
using ProjetoT.Security.Domain.Entities.Identity;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class Token
    {
        public Guid Id { get; set; }
        /* How this token was created: 'token', 'authorization_code', 'client_credentials', 'refresh' */
        public string GrantType { get; set; }
        /* Access, Refresh */
        public string TokenType { get; set; }
        /* The raw value of a token. */
        public string Value { get; set; }
        /* Rate limit for this token, which is independant, but lower than, the rate limit of the client that its authenticated to. */
        public RateLimit RateLimit { get; set; }
        /* Entity Framework Foreign Key Anchors for OAuth Clients */
        public Guid ClientApplicationId { get; set; }
        public ClientApplication Client { get; set; }
        /* Entity Framework Foreign Key Anchors for Users */
        public Guid UserId { get; set; }
        public User User { get; set; }
        
    }
}
