
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly IConfiguration _config;

        public NewsArticleService(INewsArticleRepository newsArticleRepository, IConfiguration config)
        {
            _newsArticleRepository = newsArticleRepository;
            _config = config;
        }

        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            return await _newsArticleRepository.GetNewsArticlesAsync();
        }
        public async Task<(List<NewsArticle> Articles, int TotalCount)> GetPagedNewsArticlesAsync(int pageNumber, int pageSize, bool isStaff)
        {
            var (list, total) = await _newsArticleRepository.GetPagedNewsArticlesAsync(pageNumber, pageSize, isStaff);
                return (list, total);
        }

        public async Task SaveNewsArticleAsync(NewsArticle newsArticle)
        {
            await _newsArticleRepository.CreateNewsArticleAsync(newsArticle);
        }

        public async Task UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            await _newsArticleRepository.UpdateNewsArticleAsync(newsArticle);
        }

        public async Task DeleteNewsArticleAsync(string newsArticleId)
        {
            await _newsArticleRepository.DeleteNewsArticleAsync(newsArticleId);
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(string newsArticleId)
        {
            return await _newsArticleRepository.GetNewsArticleByIdAsync(newsArticleId);
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _newsArticleRepository.GetNewsArticlesByPeriodAsync(startDate, endDate);
        }

        public async Task<List<NewsArticle>> SearchNewsArticlesAsync(string searchField, string searchString, int? tagId = null)
        {
            var articles = await _newsArticleRepository.GetNewsArticlesAsync();

            // Filter by tag if tagId is provided
            if (tagId.HasValue && tagId.Value > 0)
            {
                articles = articles.Where(a => a.Tags != null && a.Tags.Any(t => t.TagId == tagId.Value)).ToList();
            }

            // Filter by search field/string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchField)
                {
                    case "Headline":
                        articles = articles.Where(a => a.Headline != null && a.Headline.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "NewsSource":
                        articles = articles.Where(a => a.NewsSource != null && a.NewsSource.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "CategoryName":
                        articles = articles.Where(a => a.Category != null && a.Category.CategoryName != null && a.Category.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default: // NewsTitle
                        articles = articles.Where(a => a.NewsTitle != null && a.NewsTitle.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                }
            }

            return articles;
        }

        public async Task<List<NewsArticle>> GetListNewsArticlesByCreatorAsync(int userId)
        {
            return await _newsArticleRepository.GetListNewsArticlesByCreatorAsync(userId);
        }
    }
}
