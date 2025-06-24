using NewsManagementMVC.Models.ViewModels.NewsArticle;

namespace FUNewsManagementRazorPages.ViewModels.NewsArticle
{
    public class NewsArticleListViewModel
    {
        public IList<NewsArticleViewModel> NewsArticle { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
