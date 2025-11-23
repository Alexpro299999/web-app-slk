using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class AddEditSubscriberModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AddEditSubscriberModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Subscriber Subscriber { get; set; } = new();

    public SelectList TariffList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        TariffList = new SelectList(_context.Tariffs, "Id", "Name");

        if (id.HasValue)
        {
            var sub = await _context.Subscribers.FindAsync(id);
            if (sub == null)
            {
                return NotFound();
            }
            Subscriber = sub;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            TariffList = new SelectList(_context.Tariffs, "Id", "Name");
            return Page();
        }

        if (Subscriber.Id == 0)
        {
            _context.Subscribers.Add(Subscriber);
        }
        else
        {
            _context.Attach(Subscriber).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Subscribers");
    }
}