using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.NewsArticles
                    .Include(na => na.Category)
                    .Include(na => na.Tags)
                    .Include(n => n.CreatedBy)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticlesAsync: " + ex.Message);
            }
        }

        public async Task<(List<NewsArticle> Articles, int TotalCount)> GetPagedNewsArticlesAsync(int pageNumber, int pageSize, bool isStaff)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var query = context.NewsArticles
                    .Include(na => na.Category)
                    .Include(na => na.Tags)
                    .Include(n => n.CreatedBy)
                    .OrderByDescending(n => n.CreatedDate).AsQueryable();

                if (!isStaff)
                    query = query.Where(n => n.NewsStatus == true);

                int totalCount = await query.CountAsync();
                var articles = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (articles, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetPagedNewsArticlesAsync: " + ex.Message);
            }
        }


        public async Task CreateNewsArticleAsync(NewsArticle newsArticle)
        {
            Console.WriteLine($"News article: {newsArticle}");
            try
            {
                using var context = new FunewsManagementContext();

                if (newsArticle.Tags != null && newsArticle.Tags.Any())
                {
                    var tagIds = newsArticle.Tags.Select(t => t.TagId).ToList();
                    var existingTags = await context.Tags
                        .Where(t => tagIds.Contains(t.TagId))
                        .ToListAsync();
                    newsArticle.Tags = existingTags;
                }

                await context.NewsArticles.AddAsync(newsArticle);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveNewsArticleAsync: " + ex.ToString());
            }
        }

        public async Task UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            try
            {
                using var context = new FunewsManagementContext();

                var existingArticle = await context.NewsArticles
                    .Include(na => na.Tags)
                    .FirstOrDefaultAsync(na => na.NewsArticleId == newsArticle.NewsArticleId);

                if (existingArticle == null)
                    throw new Exception("NewsArticle not found.");

                var createdDate = existingArticle.CreatedDate;
                var createdById = existingArticle.CreatedById;

                context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);

                existingArticle.CreatedDate = createdDate;
                existingArticle.CreatedById = createdById;

                var newTagIds = newsArticle.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();
                var currentTagIds = existingArticle.Tags.Select(t => t.TagId).ToList();

                var tagsToRemove = existingArticle.Tags.Where(t => !newTagIds.Contains(t.TagId)).ToList();
                foreach (var tag in tagsToRemove)
                {
                    existingArticle.Tags.Remove(tag);
                }

                var tagsToAddIds = newTagIds.Except(currentTagIds).ToList();
                if (tagsToAddIds.Any())
                {
                    var tagsToAdd = await context.Tags.Where(t => tagsToAddIds.Contains(t.TagId)).ToListAsync();
                    foreach (var tag in tagsToAdd)
                    {
                        existingArticle.Tags.Add(tag);
                    }
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateNewsArticleAsync: " + ex.Message);
            }
        }

        public async Task DeleteNewsArticleAsync(string newsArticleId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var newsArticle = await context.NewsArticles
                    .Include(na => na.Tags)
                    .FirstOrDefaultAsync(na => na.NewsArticleId == newsArticleId);

                if (newsArticle != null)
                {
                    newsArticle.Tags.Clear();
                    context.NewsArticles.Remove(newsArticle);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteNewsArticleAsync: " + ex.Message);
            }
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(string newsArticleId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.Tags)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.UpdatedBy)
                    .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticleId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticleByIdAsync: " + ex.Message);
            }
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.NewsArticles
                    .Include(na => na.Category)
                    .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticlesByPeriodAsync: " + ex.Message);
            }
        }

        public async Task<List<NewsArticle>> GetListNewsArticlesByCreatorAsync(int creatorId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.NewsArticles
                    .Include(n => n.Category)
                    .Where(n => n.CreatedById == creatorId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetListNewsArticlesByCreatorAsync: " + ex.Message);
            }
        }
    }
}
