namespace MoneyManager.DataAccess.DTOs;

public record UserTransactionsDto(string AssetName, string SubcategoryName, string ParentCategoryName, decimal Amount, DateTime Date, string? Comment);