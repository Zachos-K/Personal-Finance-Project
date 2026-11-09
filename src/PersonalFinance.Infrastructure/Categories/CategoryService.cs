using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Categories;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Persistence;

namespace PersonalFinance.Infrastructure.Categories;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        Guid userId)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync();
    }

    public async Task<CategoryResponse?> GetByIdAsync(
        Guid userId,
        Guid categoryId)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(category =>
                category.Id == categoryId &&
                category.UserId == userId)
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            })
            .SingleOrDefaultAsync();
    }

    public async Task<CategoryResponse> CreateAsync(
        Guid userId,
        CreateCategoryRequest request)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Category name cannot be empty.");
        }

        var alreadyExists =
            await _dbContext.Categories.AnyAsync(category =>
                category.UserId == userId &&
                category.Name == name);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "A category with this name already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name
        };

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryResponse?> UpdateAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request)
    {
        var category =
            await _dbContext.Categories
                .SingleOrDefaultAsync(category =>
                    category.Id == categoryId &&
                    category.UserId == userId);

        if (category is null)
        {
            return null;
        }

        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Category name cannot be empty.");
        }

        var duplicateExists =
            await _dbContext.Categories.AnyAsync(other =>
                other.UserId == userId &&
                other.Name == name &&
                other.Id != categoryId);

        if (duplicateExists)
        {
            throw new InvalidOperationException(
                "A category with this name already exists.");
        }

        category.Name = name;

        await _dbContext.SaveChangesAsync();

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid categoryId)
    {
        var category =
            await _dbContext.Categories
                .SingleOrDefaultAsync(category =>
                    category.Id == categoryId &&
                    category.UserId == userId);

        if (category is null)
        {
            return false;
        }

        var hasExpenses =
            await _dbContext.Expenses.AnyAsync(expense =>
                expense.UserId == userId &&
                expense.CategoryId == categoryId);

        if (hasExpenses)
        {
            throw new InvalidOperationException(
                "Category cannot be deleted because it contains expenses.");
        }

        _dbContext.Categories.Remove(category);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}