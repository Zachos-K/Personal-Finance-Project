using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Expenses;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Expenses;

public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext _dbContext;

    public ExpenseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(
        Guid userId)
    {
        return await _dbContext.Expenses
            .AsNoTracking()
            .Where(expense => expense.UserId == userId)
            .OrderByDescending(expense => expense.Date)
            .Select(expense => new ExpenseResponse
            {
                Id = expense.Id,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category.Name,
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                CreatedAt = expense.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ExpenseResponse?> GetByIdAsync(
        Guid userId,
        Guid expenseId)
    {
        return await _dbContext.Expenses
            .AsNoTracking()
            .Where(expense =>
                expense.Id == expenseId &&
                expense.UserId == userId)
            .Select(expense => new ExpenseResponse
            {
                Id = expense.Id,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category.Name,
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                CreatedAt = expense.CreatedAt
            })
            .SingleOrDefaultAsync();
    }

    public async Task<ExpenseResponse> CreateAsync(
        Guid userId,
        CreateExpenseRequest request)
    {
        ValidateExpense(
            request.CategoryId,
            request.Amount,
            request.Description,
            request.Date);

        var category =
            await _dbContext.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(category =>
                    category.Id == request.CategoryId &&
                    category.UserId == userId);

        if (category is null)
        {
            throw new KeyNotFoundException(
                "Category was not found.");
        }

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = category.Id,
            Amount = request.Amount,
            Description = request.Description.Trim(),
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Expenses.Add(expense);

        await _dbContext.SaveChangesAsync();

        return new ExpenseResponse
        {
            Id = expense.Id,
            CategoryId = expense.CategoryId,
            CategoryName = category.Name,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            CreatedAt = expense.CreatedAt
        };
    }

    public async Task<ExpenseResponse?> UpdateAsync(
        Guid userId,
        Guid expenseId,
        UpdateExpenseRequest request)
    {
        ValidateExpense(
            request.CategoryId,
            request.Amount,
            request.Description,
            request.Date);

        var expense =
            await _dbContext.Expenses
                .SingleOrDefaultAsync(expense =>
                    expense.Id == expenseId &&
                    expense.UserId == userId);

        if (expense is null)
        {
            return null;
        }

        var category =
            await _dbContext.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(category =>
                    category.Id == request.CategoryId &&
                    category.UserId == userId);

        if (category is null)
        {
            throw new KeyNotFoundException(
                "Category was not found.");
        }

        expense.CategoryId = category.Id;
        expense.Amount = request.Amount;
        expense.Description = request.Description.Trim();
        expense.Date = request.Date;

        await _dbContext.SaveChangesAsync();

        return new ExpenseResponse
        {
            Id = expense.Id,
            CategoryId = expense.CategoryId,
            CategoryName = category.Name,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            CreatedAt = expense.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid expenseId)
    {
        var expense =
            await _dbContext.Expenses
                .SingleOrDefaultAsync(expense =>
                    expense.Id == expenseId &&
                    expense.UserId == userId);

        if (expense is null)
        {
            return false;
        }

        _dbContext.Expenses.Remove(expense);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static void ValidateExpense(
        Guid categoryId,
        decimal amount,
        string description,
        DateTime date)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Category is required.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException(
                "Amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Description cannot be empty.");
        }

        if (date == default)
        {
            throw new ArgumentException(
                "Date is required.");
        }
    }
}