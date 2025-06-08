using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategoriesAsync: " + ex.Message);
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.NewsArticles)
                    .FirstOrDefaultAsync(c => c.CategoryId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategoryByIdAsync: " + ex.Message);
            }
        }

        public async Task CreateCategoryAsync(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveCategoryAsync: " + ex.Message);
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateCategoryAsync: " + ex.Message);
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                using var context = new FunewsManagementContext();

                var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
                if (category == null)
                {
                    throw new Exception("Category not found");
                }

                var childCategories = await context.Categories
                    .Where(c => c.ParentCategoryId == id)
                    .ToListAsync();

                foreach (var child in childCategories)
                {
                    child.ParentCategoryId = null;
                }

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteCategoryAsync: " + ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
