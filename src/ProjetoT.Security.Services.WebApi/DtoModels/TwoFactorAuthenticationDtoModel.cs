namespace ProjetoT.Security.Services.WebApi.DtoModels
{
    public class TwoFactorAuthenticationDtoModel
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }
}