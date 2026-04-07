namespace MoneyManager.DataAccess.Entities;

public class Transaction : EntityBase
{
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid AssetId { get; set; }
    public string? Comment { get; set; }
}