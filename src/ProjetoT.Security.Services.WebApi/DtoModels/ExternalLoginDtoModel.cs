using System.ComponentModel.DataAnnotations;

namespace ProjetoT.Security.Services.WebApi.DtoModels
{
    public class ExternalLoginDtoModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}