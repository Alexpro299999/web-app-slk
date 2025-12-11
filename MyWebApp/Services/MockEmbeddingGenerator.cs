using Pgvector;
using System.Security.Cryptography;
using System.Text;

namespace MyWebApp.Services;

public class MockEmbeddingGenerator : IEmbeddingGenerator
{
    public Vector GenerateEmbedding(string text)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(text.ToLowerInvariant());
        var hashBytes = md5.ComputeHash(inputBytes);

        var seed = BitConverter.ToInt32(hashBytes, 0);
        var random = new Random(seed);

        var vector = new float[3];
        for (int i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)random.NextDouble();
        }
        return new Vector(vector);
    }
}