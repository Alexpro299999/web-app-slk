using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class EquipmentsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EquipmentsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Equipment> Equipments { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Equipments != null)
        {
            Equipments = await _context.Equipments.ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var eq = await _context.Equipments.FindAsync(id);
        if (eq != null)
        {
            _context.Equipments.Remove(eq);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}