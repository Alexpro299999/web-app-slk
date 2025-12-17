using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public SelectList? AvailableTables { get; set; }

    [BindProperty]
    public string ColumnName { get; set; } = string.Empty;

    [BindProperty]
    public string ColumnType { get; set; } = "string";

    [BindProperty]
    public int? LinkedTableId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Table = await _service.GetTableSchemaAsync(id);
        if (Table == null) return NotFound();

        var tables = await _service.GetTablesAsync();
        AvailableTables = new SelectList(tables.Where(t => t.Id != id), "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!string.IsNullOrWhiteSpace(ColumnName))
        {
            await _service.AddColumnAsync(id, ColumnName, ColumnType, LinkedTableId);
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteColumnAsync(int id, int colId)
    {
        await _service.DeleteColumnAsync(colId);
        return RedirectToPage(new { id });
    }
}