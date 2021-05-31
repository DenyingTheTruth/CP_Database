using System.Threading.Tasks;

namespace forest_report_api.Service
{
    public interface IMailer
    {
        Task SendEmailAsync(string email, string message);
    }
}