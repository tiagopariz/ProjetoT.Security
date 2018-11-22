using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class RedirectUri
    {

        [Key]
        public Guid Id { get; set; }
        /*  These are the Foreign Key anchors that, combined with the ClientApplication.RedirectURIs field, lets EntityFramework know that this is a (1 : Many) mapping */
        public Guid ClientApplicationId { get; set; }
        public ClientApplication ClientApplication { get; set; }
        public string URI { get; set; }
    }
}
