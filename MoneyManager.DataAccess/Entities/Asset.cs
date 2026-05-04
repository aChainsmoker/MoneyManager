namespace MoneyManager.DataAccess.Entities;

public class Asset : EntityBase
{
    public string Name { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}