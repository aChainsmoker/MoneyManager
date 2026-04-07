namespace MoneyManager.DataAccess.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = null!;
    public CategoryType Type { get; set; }
    public Guid? ParentId { get; set; }
}