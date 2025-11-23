using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class AddEditTariffModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AddEditTariffModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Tariff Tariff { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff == null)
            {
                return NotFound();
            }
            Tariff = tariff;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Tariff.Id == 0)
        {
            _context.Tariffs.Add(Tariff);
        }
        else
        {
            _context.Attach(Tariff).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("/Tariffs");
    }
}