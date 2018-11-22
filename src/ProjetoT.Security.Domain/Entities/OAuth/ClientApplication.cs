using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjetoT.Security.Domain.Entities.Identity;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class ClientApplication
    {

        /* EntityFramework classes that have an Id field that deviates from the auto-detectable formats need to have that field annotated with [Key] */
        [Key]
        public Guid Id { get; set; }
        /* Each App needs a Client Secret, but it is assigned at creation */
        [Required]
        public string ClientSecret { get; set; }
        /* Each App Needs an Owner, which will be assigned at creation. This is also a Foreign Key to the Users table. */
        [Required]
        [ForeignKey("Id")]
        public User Owner { get; set; }
        /* This field, combined with the RedirectURI.ClientApplication field, lets EntityFramework know that this is a (1 : Many) mapping */
        public List<RedirectUri> RedirectUris { get; set; } = new List<RedirectUri>();
        /*  Like above, this notifies EntityFramework of another (1 : Many) mapping */
        public List<Token> UserTokens { get; set; } = new List<Token>();
        /* A Rate limit object for our client - separate from any rate limits applied to the users of this application. */
        public RateLimit RateLimit { get; set; }
        /* A rate limit objects for tokens issued to this client - usually null
         * but if a client has been granted special overrides, the limits specified here will be issued to the tokens, 
         * as opposed to the default grant_type token limits.
         * This allows us to offer specific applications increased overall limits, and increased per-user limits, if so desired. */
        public RateLimit SubordinateTokenLimits { get; set; }
        /* Each App needs a Name, which is submitted by the user at Creation time */
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string ClientName { get; set; }
        /* Each App needs a Description, which is submitted by the user at Edit time */
        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string ClientDescription { get; set; }
    }
}
