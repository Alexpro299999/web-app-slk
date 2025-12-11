using Pgvector;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public string SerialNumber { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsInStock { get; set; }

        [Column(TypeName = "vector(384)")]
        public Vector? Embedding { get; set; }
    }
}