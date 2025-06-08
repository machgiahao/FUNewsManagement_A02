using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementSystem.DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        // Use dependency injection for the repository
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetCategoriesAsync();
        }

        public async Task DeleteCategoryAsync(int newsArticleId)
        {
            await _categoryRepository.DeleteCategoryAsync(newsArticleId);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _categoryRepository.CreateCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetCategoryByIdAsync(id);
        }
    }
}
