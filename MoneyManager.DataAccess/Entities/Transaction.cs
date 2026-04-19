namespace MoneyManager.DataAccess.Entities;

public class Transaction : EntityBase
{
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; } = null!;
    public string? Comment { get; set; }
}