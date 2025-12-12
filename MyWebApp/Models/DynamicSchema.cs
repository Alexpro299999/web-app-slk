using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models;

public class MetaTable
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<MetaColumn> Columns { get; set; } = new();
}

public class MetaColumn
{
    [Key]
    public int Id { get; set; }
    public int MetaTableId { get; set; }
    [ForeignKey("MetaTableId")]
    public MetaTable? Table { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string DataType { get; set; } = "string";
    public int? LinkedTableId { get; set; }
}

public class MetaRow
{
    [Key]
    public int Id { get; set; }
    public int MetaTableId { get; set; }
    [ForeignKey("MetaTableId")]
    public MetaTable? Table { get; set; }
    public List<MetaValue> Values { get; set; } = new();
}

public class MetaValue
{
    [Key]
    public int Id { get; set; }
    public int MetaRowId { get; set; }
    [ForeignKey("MetaRowId")]
    public MetaRow? Row { get; set; }
    public int MetaColumnId { get; set; }
    [ForeignKey("MetaColumnId")]
    public MetaColumn? Column { get; set; }
    public string? Value { get; set; }
}