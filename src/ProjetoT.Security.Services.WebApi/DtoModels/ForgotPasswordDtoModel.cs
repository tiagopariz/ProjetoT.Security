using System.ComponentModel.DataAnnotations;

namespace ProjetoT.Security.Services.WebApi.DtoModels
{
    public class ForgotPasswordDtoModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}