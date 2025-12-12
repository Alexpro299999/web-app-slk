using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Services;

public class DynamicDbService
{
    private readonly ApplicationDbContext _context;

    public DynamicDbService(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task AddRowAsync(int tableId, Dictionary<int, string> values)
    {
        var row = new MetaRow { MetaTableId = tableId };
        _context.MetaRows.Add(row);
        await _context.SaveChangesAsync();

        foreach (var kvp in values)
        {
            _context.MetaValues.Add(new MetaValue
            {
                MetaRowId = row.Id,
                MetaColumnId = kvp.Key,
                Value = kvp.Value
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRowAsync(int rowId)
    {
        var row = await _context.MetaRows.FindAsync(rowId);
        if (row != null)
        {
            _context.MetaRows.Remove(row);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Dictionary<string, string>>> GetTableDataAsync(int tableId)
    {
        var columns = await _context.MetaColumns
            .Where(c => c.MetaTableId == tableId)
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var rows = await _context.MetaRows
            .Where(r => r.MetaTableId == tableId)
            .Include(r => r.Values)
            .ToListAsync();

        var result = new List<Dictionary<string, string>>();

        foreach (var row in rows)
        {
            var rowData = new Dictionary<string, string>
            {
                ["_Id"] = row.Id.ToString()
            };

            foreach (var col in columns)
            {
                var val = row.Values.FirstOrDefault(v => v.MetaColumnId == col.Key);
                rowData[col.Value] = val?.Value ?? string.Empty;
            }
            result.Add(rowData);
        }

        return result;
    }

    public async Task<List<MetaTable>> GetTablesAsync()
    {
        return await _context.MetaTables.Include(t => t.Columns).ToListAsync();
    }

    public async Task<MetaTable?> GetTableSchemaAsync(int id)
    {
        return await _context.MetaTables.Include(t => t.Columns).FirstOrDefaultAsync(t => t.Id == id);
    }
}