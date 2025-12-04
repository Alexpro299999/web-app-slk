using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;
using Pgvector.EntityFrameworkCore;

namespace MyWebApp.Pages
{
    public class EquipmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmbeddingGenerator _embeddingGenerator;

        public EquipmentsModel(ApplicationDbContext context, IEmbeddingGenerator embeddingGenerator)
        {
            _context = context;
            _embeddingGenerator = embeddingGenerator;
        }

        public IList<Equipment> Equipment { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                var queryVector = _embeddingGenerator.GenerateEmbedding(SearchString);

                Equipment = await _context.Equipments
                    .OrderBy(e => e.Embedding!.L2Distance(queryVector))
                    .ToListAsync();
            }
            else
            {
                Equipment = await _context.Equipments.ToListAsync();
            }
        }
    }
}