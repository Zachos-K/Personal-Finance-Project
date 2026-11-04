using PersonalFinance.Application.Auth;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Interfaces;

public interface IJwtTokenGenerator
{
    TokenResult GenerateToken(User user);
}