using PersonalFinance.Application.Categories;

namespace PersonalFinance.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(Guid userId);

    Task<CategoryResponse?> GetByIdAsync(
        Guid userId,
        Guid categoryId);

    Task<CategoryResponse> CreateAsync(
        Guid userId,
        CreateCategoryRequest request);

    Task<CategoryResponse?> UpdateAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid categoryId);
}