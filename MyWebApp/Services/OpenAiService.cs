using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyWebApp.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl = "http://localhost:11434/v1/embeddings";
        private readonly string _modelName = "all-minilm"; 

        public OpenAiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ollama");
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var requestBody = new
            {
                model = _modelName,
                input = text
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Local AI Error ({response.StatusCode}): {error}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);
            var embeddingElement = doc.RootElement.GetProperty("data")[0].GetProperty("embedding");

            var floats = new List<float>();
            foreach (var item in embeddingElement.EnumerateArray())
            {
                floats.Add(item.GetSingle());
            }

            return floats.ToArray();
        }
    }
}