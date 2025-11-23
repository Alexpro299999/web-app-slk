using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class TariffsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public TariffsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Tariff> Tariffs { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Tariffs != null)
        {
            Tariffs = await _context.Tariffs.ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var tariff = await _context.Tariffs.FindAsync(id);
        if (tariff != null)
        {
            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}