using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Dynamic;

public class EditColumnModel : PageModel
{
    private readonly DynamicDbService _service;

    public EditColumnModel(DynamicDbService service)
    {
        _service = service;
    }

    public MetaColumn? Column { get; set; }
    public SelectList? AvailableTables { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public int? LinkedTableId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Column = await _service.GetColumnAsync(id);
        if (Column == null) return NotFound();

        Name = Column.Name;
        LinkedTableId = Column.LinkedTableId;

        if (Column.DataType == "relation")
        {
            var tables = await _service.GetTablesAsync();
            AvailableTables = new SelectList(tables.Where(t => t.Id != Column.MetaTableId), "Id", "Name", LinkedTableId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _service.UpdateColumnAsync(id, Name, LinkedTableId);
        var col = await _service.GetColumnAsync(id);
        return RedirectToPage("Structure", new { id = col?.MetaTableId });
    }
}