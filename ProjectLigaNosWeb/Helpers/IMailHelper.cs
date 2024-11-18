using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Helpers
{
    public interface IMailHelper
    {
        Response SendEmail(string to, string subject, string body);

    }
}
