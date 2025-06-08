using FUNewsManagement.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetNewsArticlesAsync();
        Task SaveNewsArticleAsync(NewsArticle newsArticle);
        Task UpdateNewsArticleAsync(NewsArticle newsArticle);
        Task DeleteNewsArticleAsync(string newsArticleId);
        Task<NewsArticle?> GetNewsArticleByIdAsync(string newsArticleId);
        Task<List<NewsArticle>> GetNewsArticlesByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<List<NewsArticle>> SearchNewsArticlesAsync(string searchField, string searchString, int? tagId = null);
        Task<List<NewsArticle>> GetListNewsArticlesByCreatorAsync(int userId);
    }
}
