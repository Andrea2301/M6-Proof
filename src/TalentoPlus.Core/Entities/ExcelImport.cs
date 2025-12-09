namespace TalentoPlus.Core.Entities;

public class ExcelImport
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public DateTime ImportDate { get; set; }
    public string ImportedBy { get; set; }
    public string? ErrorLog { get; set; }
}