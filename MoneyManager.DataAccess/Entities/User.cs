namespace MoneyManager.DataAccess.Entities;

public class User : EntityBase
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string Salt { get; set; } = null!;
}