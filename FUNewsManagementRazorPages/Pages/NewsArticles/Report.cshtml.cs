using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementRazorPages.ViewModels.NewsArticle;
using FUNewsManagementSystem.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementRazorPages.Pages.NewsArticles
{
    public class ReportModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public ReportModel(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        [BindProperty(SupportsGet = true)]
        public ReportViewModel ReportViewModel { get; set; } = new();

        public List<NewsArticle> NewsArticles { get; set; } = new();
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public List<string> CategoryLabels { get; set; } = new();
        public List<int> CategoryCounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (ReportViewModel.StartDate.HasValue && ReportViewModel.EndDate.HasValue)
            {
                NewsArticles = await _newsArticleRepository.GetNewsArticlesByPeriodAsync(
                    ReportViewModel.StartDate.Value,
                    ReportViewModel.EndDate.Value
                );
                if (NewsArticles != null && NewsArticles.Any())
                {
                    ActiveCount = NewsArticles.Count(a => a.NewsStatus == true);
                    InactiveCount = NewsArticles.Count(a => a.NewsStatus != true);

                    var categoryGroups = NewsArticles
                        .GroupBy(a => a.Category?.CategoryName)
                        .Select(g => new { Category = g.Key, Count = g.Count() })
                        .ToList();

                    CategoryLabels = categoryGroups.Select(g => g.Category ?? "Uncategorized").ToList();
                    CategoryCounts = categoryGroups.Select(g => g.Count).ToList();
                }
            }
        }

    }
}
