using PersonalFinance.Application.Budgets;

namespace PersonalFinance.Application.Interfaces;

public interface IBudgetService
{
    Task<BudgetResponse?> GetAsync(
        Guid userId,
        int year,
        int month);

    Task<BudgetResponse> SetAsync(
        Guid userId,
        int year,
        int month,
        SetBudgetRequest request);

    Task<bool> DeleteAsync(
        Guid userId,
        int year,
        int month);
        
}