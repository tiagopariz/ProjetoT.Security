using System.Threading.Tasks;

namespace ProjetoT.Security.Infra.Messages.Email.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}