using System.Collections.Generic;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class AuthorizedApps
    {
        public IList<ClientApplication> Apps { get; set; }
    }
}
