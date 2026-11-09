using PersonalFinance.Application.Expenses;

namespace PersonalFinance.Application.Interfaces;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseResponse>> GetAllAsync( Guid userId);

    Task<ExpenseResponse?> GetByIdAsync(
        Guid userId,
        Guid expenseId);

    Task<ExpenseResponse> CreateAsync(
        Guid userId,
        CreateExpenseRequest request);

    Task<ExpenseResponse?> UpdateAsync(
        Guid userId,
        Guid expenseId,
        UpdateExpenseRequest request);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid expenseId);
}