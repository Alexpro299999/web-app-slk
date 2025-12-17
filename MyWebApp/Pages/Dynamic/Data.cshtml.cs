using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Dynamic;

public class DataModel : PageModel
{
    private readonly DynamicDbService _service;

    public DataModel(DynamicDbService service)
    {
        _service = service;
    }

    public MetaTable? Table { get; set; }
    public List<Dictionary<string, string>> Rows { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Table = await _service.GetTableSchemaAsync(id);
        if (Table == null) return NotFound();

        Rows = await _service.GetTableDataAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, int rowId)
    {
        try
        {
            await _service.DeleteRowAsync(rowId);
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }
        return RedirectToPage(new { id });
    }
}