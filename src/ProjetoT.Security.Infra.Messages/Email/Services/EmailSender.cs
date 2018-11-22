using ProjetoT.Security.Infra.Messages.Email.Interfaces;
using System.Threading.Tasks;

namespace ProjetoT.Security.Infra.Messages.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}
