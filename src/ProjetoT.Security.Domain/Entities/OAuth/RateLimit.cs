using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class RateLimit
    {
        public Guid Id { get; set; } // Primary key for Entity Framework, because this will also be a database object
        public int? Limit { get; set; } // Nullable, so that a limit of 'null' may represent no limit at all.
        public TimeSpan? Window { get; set; } // The timespan of the rolling window. 
        public Guid TokenId { get; set; }
        public Token Token { get; set; }
        public Guid ClientId { get; set; }
        public ClientApplication Client { get; set; }
        public Guid SubordinatedClientId { get; set; }
        public ClientApplication SubordinatedClient { get; set; }

        public static RateLimit DefaultClientLimit =>
            new RateLimit()
            {
                Limit = 5, // 10_000
                Window = TimeSpan.FromHours(1),
            };

        public static RateLimit DefaultImplicitLimit =>
            new RateLimit()
            {
                Limit = 1, // 150
                Window = TimeSpan.FromHours(1)
            };

        public static RateLimit DefaultAuthorizationCodeLimit =>
            new RateLimit()
            {
                Limit = 500,
                Window = TimeSpan.FromHours(1)
            };
    }
}