using Pgvector;

namespace MyWebApp.Services;

public interface IEmbeddingGenerator
{
    Vector GenerateEmbedding(string text);
}