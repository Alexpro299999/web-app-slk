
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Dynamic;

public class StructureModel : PageModel
{
    private readonly DynamicDbService _service;

    public StructureModel(DynamicDbService service)
    {
        _service = service;
    }

    public MetaTable? Table { get; set; }

    [BindProperty]
    public string ColumnName { get; set; } = string.Empty;

    [BindProperty]
    public string ColumnType { get; set; } = "string";

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Table = await _service.GetTableSchemaAsync(id);
        if (Table == null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!string.IsNullOrWhiteSpace(ColumnName))
        {
            await _service.AddColumnAsync(id, ColumnName, ColumnType);
        }
        return RedirectToPage(new { id });
    }
}