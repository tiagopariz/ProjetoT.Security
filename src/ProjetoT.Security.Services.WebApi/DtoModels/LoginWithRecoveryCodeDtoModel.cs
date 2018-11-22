using System.ComponentModel.DataAnnotations;

namespace ProjetoT.Security.Services.WebApi.DtoModels
{
    public class LoginWithRecoveryCodeDtoModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}