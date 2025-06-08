
using FUNewsManagement.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public interface INewsArticleRepository
    {
        Task<List<NewsArticle>> GetNewsArticlesAsync();
        Task CreateNewsArticleAsync(NewsArticle newsArticle);
        Task UpdateNewsArticleAsync(NewsArticle newsArticle);
        Task DeleteNewsArticleAsync(string newsArticleId);
        Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId);
        Task<List<NewsArticle>> GetNewsArticlesByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<List<NewsArticle>> GetListNewsArticlesByCreatorAsync(int userId);
    }
}
