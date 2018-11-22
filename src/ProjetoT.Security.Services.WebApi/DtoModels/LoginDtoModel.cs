using System.ComponentModel.DataAnnotations;

namespace ProjetoT.Security.Services.WebApi.DtoModels
{
    public class LoginDtoModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}