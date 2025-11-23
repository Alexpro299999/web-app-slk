using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class AddEditEquipmentModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AddEditEquipmentModel(ApplicationDbContext context)
    {
        _context = context;
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