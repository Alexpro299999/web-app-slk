using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages;

public class AddEditEquipmentModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IEmbeddingGenerator _embeddingGenerator;

    public AddEditEquipmentModel(ApplicationDbContext context, IEmbeddingGenerator embeddingGenerator)
    {
        _context = context;
        _embeddingGenerator = embeddingGenerator;
    }

    [BindProperty]
    public Equipment Equipment { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var eq = await _context.Equipments.FindAsync(id);
            if (eq == null)
            {
                return NotFound();
            }
            Equipment = eq;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var textToEmbed = $"{Equipment.ModelName} {Equipment.Type}";
        Equipment.Embedding = _embeddingGenerator.GenerateEmbedding(textToEmbed);

        if (Equipment.Id == 0)
        {
            _context.Equipments.Add(Equipment);
        }
        else
        {
            _context.Attach(Equipment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Equipments");
    }
}