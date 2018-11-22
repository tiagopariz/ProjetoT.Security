using System;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class Authorize
    {
        public string ClientName { get; set; }

        public Guid ClientId { get; set; }

        public string ClientDescription { get; set; }

        public string ResponseType { get; set; }

        public string RedirectUri { get; set; }

        public string[] Scopes { get; set; } = new string[0];

        public string State { get; set; }
    }
}