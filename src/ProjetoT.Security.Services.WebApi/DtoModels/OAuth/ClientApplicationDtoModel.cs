using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoT.Security.Services.WebApi.DtoModels.OAuth
{
    public class ClientApplicationDtoModel
    {
        public Guid Id { get; internal set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string ClientName { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string ClientDescription { get; set; }

        public string ClientSecret { get; internal set; }

        public string[] RedirectUris { get; set; } = new string[0];
    }
}