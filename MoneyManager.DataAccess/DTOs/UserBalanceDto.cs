namespace MoneyManager.DataAccess.DTOs;

public record UserBalanceDto(Guid Id, string Name, string Email, decimal Balance) : UserInfoDto(Id, Name, Email);