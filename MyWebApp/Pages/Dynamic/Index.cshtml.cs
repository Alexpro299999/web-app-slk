using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Dynamic;

public class IndexModel : PageModel
{
    private readonly DynamicDbService _service;

    public IndexModel(DynamicDbService service)
    {
        _service = service;
    }

    public List<MetaTable> Tables { get; set; } = new();

    [BindProperty]
    public string NewTableName { get; set; } = string.Empty;

    [BindProperty]
    public string NewTableDesc { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        Tables = await _service.GetTablesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewTableName))
        {
            await _service.CreateTableAsync(NewTableName, NewTableDesc);
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _service.DeleteTableAsync(id);
        return RedirectToPage();
    }
}