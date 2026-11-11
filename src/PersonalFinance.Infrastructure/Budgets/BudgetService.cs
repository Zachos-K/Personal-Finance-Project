using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Budgets;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Budgets;

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _dbContext;

    public BudgetService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BudgetResponse?> GetAsync(
        Guid userId,
        int year,
        int month)
    {
        ValidateYearAndMonth(year, month);

        return await _dbContext.Budgets
            .AsNoTracking()
            .Where(budget =>
                budget.UserId == userId &&
                budget.Year == year &&
                budget.Month == month)
            .Select(budget => new BudgetResponse
            {
                Id = budget.Id,
                Year = budget.Year,
                Month = budget.Month,
                Amount = budget.Amount
            })
            .SingleOrDefaultAsync();
    }

    public async Task<BudgetResponse> SetAsync(
        Guid userId,
        int year,
        int month,
        SetBudgetRequest request)
    {
        ValidateYearAndMonth(year, month);

        if (request.Amount <= 0)
        {
            throw new ArgumentException(
                "Budget amount must be greater than zero.");
        }

        var budget =
            await _dbContext.Budgets
                .SingleOrDefaultAsync(budget =>
                    budget.UserId == userId &&
                    budget.Year == year &&
                    budget.Month == month);

        if (budget is null)
        {
            budget = new Budget
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Year = year,
                Month = month,
                Amount = request.Amount
            };

            _dbContext.Budgets.Add(budget);
        }
        else
        {
            budget.Amount = request.Amount;
        }

        await _dbContext.SaveChangesAsync();

        return new BudgetResponse
        {
            Id = budget.Id,
            Year = budget.Year,
            Month = budget.Month,
            Amount = budget.Amount
        };
    }

    public async Task<bool> DeleteAsync(
        Guid userId,
        int year,
        int month)
    {
        ValidateYearAndMonth(year, month);

        var budget =
            await _dbContext.Budgets
                .SingleOrDefaultAsync(budget =>
                    budget.UserId == userId &&
                    budget.Year == year &&
                    budget.Month == month);

        if (budget is null)
        {
            return false;
        }

        _dbContext.Budgets.Remove(budget);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static void ValidateYearAndMonth(
        int year,
        int month)
    {
        if (year < 2000 || year > 2100)
        {
            throw new ArgumentException(
                "Year must be between 2000 and 2100.");
        }

        if (month < 1 || month > 12)
        {
            throw new ArgumentException(
                "Month must be between 1 and 12.");
        }
    }
}