using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Dynamic;

public class EditRowModel : PageModel
{
    private readonly DynamicDbService _service;

    public EditRowModel(DynamicDbService service)
    {
        _service = service;
    }

    public MetaTable? Table { get; set; }
    public Dictionary<int, string> CurrentValues { get; set; } = new();
    public Dictionary<int, Dictionary<int, string>> RelationsData { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int tableId, int rowId)
    {
        Table = await _service.GetTableSchemaAsync(tableId);
        if (Table == null) return NotFound();

        CurrentValues = await _service.GetRowValuesAsync(rowId);

        foreach (var col in Table.Columns.Where(c => c.DataType == "relation" && c.LinkedTableId.HasValue))
        {
            RelationsData[col.Id] = await _service.GetLookupOptionsAsync(col.LinkedTableId.Value);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int tableId, int rowId)
    {
        var values = new Dictionary<int, string>();

        foreach (var key in Request.Form.Keys)
        {
            if (key.StartsWith("Values["))
            {
                var idStr = key.Replace("Values[", "").Replace("]", "");
                if (int.TryParse(idStr, out int colId))
                {
                    values[colId] = Request.Form[key].ToString();
                }
            }
        }

        var files = Request.Form.Files;
        await _service.UpdateRowAsync(rowId, values, files);

        return RedirectToPage("Data", new { id = tableId });
    }
}