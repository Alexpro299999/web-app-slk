using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Services;

public class DynamicDbService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DynamicDbService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<MetaTable> CreateTableAsync(string name, string? description)
    {
        var table = new MetaTable { Name = name, Description = description };
        _context.MetaTables.Add(table);
        await _context.SaveChangesAsync();
        return table;
    }

    public async Task DeleteTableAsync(int id)
    {
        var table = await _context.MetaTables.FindAsync(id);
        if (table != null)
        {
            _context.MetaTables.Remove(table);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddColumnAsync(int tableId, string name, string type, int? linkedTableId = null)
    {
        var column = new MetaColumn
        {
            MetaTableId = tableId,
            Name = name,
            DataType = type,
            LinkedTableId = linkedTableId
        };
        _context.MetaColumns.Add(column);
        await _context.SaveChangesAsync();
    }

    public async Task<MetaColumn?> GetColumnAsync(int id)
    {
        return await _context.MetaColumns.FindAsync(id);
    }

    public async Task UpdateColumnAsync(int id, string name, int? linkedTableId)
    {
        var col = await _context.MetaColumns.FindAsync(id);
        if (col != null)
        {
            col.Name = name;
            if (col.DataType == "relation")
            {
                col.LinkedTableId = linkedTableId;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteColumnAsync(int id)
    {
        var col = await _context.MetaColumns.FindAsync(id);
        if (col != null)
        {
            _context.MetaColumns.Remove(col);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddRowAsync(int tableId, Dictionary<int, string> values, IFormFileCollection? files = null)
    {
        var row = new MetaRow { MetaTableId = tableId };
        _context.MetaRows.Add(row);
        await _context.SaveChangesAsync();
        await SaveValuesAsync(row.Id, values, files);
    }

    public async Task UpdateRowAsync(int rowId, Dictionary<int, string> values, IFormFileCollection? files = null)
    {
        await SaveValuesAsync(rowId, values, files);
    }

    private async Task SaveValuesAsync(int rowId, Dictionary<int, string> values, IFormFileCollection? files)
    {
        foreach (var kvp in values)
        {
            var metaValue = await _context.MetaValues
                .FirstOrDefaultAsync(v => v.MetaRowId == rowId && v.MetaColumnId == kvp.Key);

            if (metaValue == null)
            {
                metaValue = new MetaValue { MetaRowId = rowId, MetaColumnId = kvp.Key };
                _context.MetaValues.Add(metaValue);
            }
            metaValue.Value = kvp.Value;
        }

        if (files != null)
        {
            foreach (var file in files)
            {
                if (int.TryParse(file.Name.Replace("file_", ""), out int colId))
                {
                    var path = await SaveFileAsync(file);

                    var metaValue = await _context.MetaValues
                        .FirstOrDefaultAsync(v => v.MetaRowId == rowId && v.MetaColumnId == colId);

                    if (metaValue == null)
                    {
                        metaValue = new MetaValue { MetaRowId = rowId, MetaColumnId = colId };
                        _context.MetaValues.Add(metaValue);
                    }
                    metaValue.Value = path;
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);
        var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return "/uploads/" + uniqueName;
    }

    public async Task DeleteRowAsync(int rowId)
    {
        var row = await _context.MetaRows.FindAsync(rowId);
        if (row == null) return;

        var referencingColumns = await _context.MetaColumns
            .Where(c => c.LinkedTableId == row.MetaTableId)
            .Include(c => c.Table)
            .ToListAsync();

        foreach (var col in referencingColumns)
        {
            var hasReference = await _context.MetaValues
                .AnyAsync(v => v.MetaColumnId == col.Id && v.Value == rowId.ToString());

            if (hasReference)
            {
                throw new InvalidOperationException($"Нельзя удалить запись #{rowId}, так как она используется в таблице '{col.Table?.Name}' (колонка '{col.Name}').");
            }
        }

        _context.MetaRows.Remove(row);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Dictionary<string, string>>> GetTableDataAsync(int tableId)
    {
        var columns = await _context.MetaColumns
            .Where(c => c.MetaTableId == tableId)
            .ToDictionaryAsync(c => c.Id, c => c);

        var rows = await _context.MetaRows
            .Where(r => r.MetaTableId == tableId)
            .Include(r => r.Values)
            .ToListAsync();

        var relationCache = new Dictionary<int, string>();
        var result = new List<Dictionary<string, string>>();

        foreach (var row in rows)
        {
            var rowData = new Dictionary<string, string>
            {
                ["_Id"] = row.Id.ToString()
            };

            foreach (var colKv in columns)
            {
                var col = colKv.Value;
                var val = row.Values.FirstOrDefault(v => v.MetaColumnId == col.Id)?.Value ?? "";

                if (col.DataType == "relation" && int.TryParse(val, out int relatedRowId))
                {
                    if (!relationCache.ContainsKey(relatedRowId))
                    {
                        relationCache[relatedRowId] = await GetRowDisplayTextAsync(relatedRowId);
                    }
                    rowData[col.Name] = relationCache[relatedRowId];
                }
                else
                {
                    rowData[col.Name] = val;
                }
            }
            result.Add(rowData);
        }

        return result;
    }

    public async Task<Dictionary<int, string>> GetRowValuesAsync(int rowId)
    {
        var values = await _context.MetaValues
            .Where(v => v.MetaRowId == rowId)
            .ToDictionaryAsync(v => v.MetaColumnId, v => v.Value ?? "");
        return values;
    }

    private async Task<string> GetRowDisplayTextAsync(int rowId)
    {
        var row = await _context.MetaRows
            .Include(r => r.Values)
            .ThenInclude(v => v.Column)
            .FirstOrDefaultAsync(r => r.Id == rowId);

        if (row == null) return $"#{rowId} (Удалено)";

        var firstStringVal = row.Values
            .OrderBy(v => v.MetaColumnId)
            .FirstOrDefault(v => v.Column?.DataType == "string")?.Value;

        return firstStringVal ?? $"#{rowId}";
    }

    public async Task<List<MetaTable>> GetTablesAsync()
    {
        return await _context.MetaTables.Include(t => t.Columns).ToListAsync();
    }

    public async Task<MetaTable?> GetTableSchemaAsync(int id)
    {
        return await _context.MetaTables.Include(t => t.Columns).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Dictionary<int, string>> GetLookupOptionsAsync(int tableId)
    {
        var rows = await _context.MetaRows
            .Where(r => r.MetaTableId == tableId)
            .Include(r => r.Values)
            .ThenInclude(v => v.Column)
            .ToListAsync();

        var options = new Dictionary<int, string>();
        foreach (var r in rows)
        {
            var display = r.Values.OrderBy(v => v.MetaColumnId).FirstOrDefault(v => v.Column?.DataType == "string")?.Value ?? $"Item #{r.Id}";
            options[r.Id] = display;
        }
        return options;
    }
}