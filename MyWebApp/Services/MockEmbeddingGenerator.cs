using Pgvector;

namespace MyWebApp.Services;

public class MockEmbeddingGenerator : IEmbeddingGenerator
{
    public Vector GenerateEmbedding(string text)
    {
        var random = new Random(text.GetHashCode());
        var vector = new float[3];
        for (int i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)random.NextDouble();
        }
        return new Vector(vector);
    }
}