using System.Threading.Tasks;

namespace MyWebApp.Services
{
    public interface IOpenAiService
    {
        Task<float[]> GetEmbeddingAsync(string text);
    }
}