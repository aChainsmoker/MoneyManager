namespace MoneyManager.DataAccess.DTOs;

public record TimePeriodIncomeExpensesDto(decimal Income, decimal Expenses, int Year, int Month);