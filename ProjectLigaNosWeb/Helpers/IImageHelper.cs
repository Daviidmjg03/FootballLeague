using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
