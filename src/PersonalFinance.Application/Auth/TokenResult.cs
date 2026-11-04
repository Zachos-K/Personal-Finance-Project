namespace PersonalFinance.Application.Auth;

public record TokenResult(
    string Token,
    DateTime ExpiresAtUtc
);