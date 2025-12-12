using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Dynamic;

public class AddRowModel : PageModel
{
    private readonly DynamicDbService _service;

    public AddRowModel(DynamicDbService service)
    {
        _service = service;
    }

    public MetaTable? Table { get; set; }

    [BindProperty]
    public Dictionary<int, string> Values { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Table = await _service.GetTableSchemaAsync(id);
        if (Table == null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _service.AddRowAsync(id, Values);
        return RedirectToPage("Data", new { id });
    }
}