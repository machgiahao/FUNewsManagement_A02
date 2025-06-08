
using FUNewsManagement.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task CreateCategoryAsync(Category Category);
        Task UpdateCategoryAsync(Category Category);
        Task DeleteCategoryAsync(int newsArticleId);
        Task<Category> GetCategoryByIdAsync(int id);
    }
}
