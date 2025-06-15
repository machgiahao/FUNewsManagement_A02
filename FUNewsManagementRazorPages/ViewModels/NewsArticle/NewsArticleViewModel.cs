using System;
namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class NewsArticleViewModel
    {
        public string NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public bool? NewsStatus { get; set; }
        public string CategoryName { get; set; }
        public string? CreatedBy { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<int> SelectedTagIds { get; set; } = new();
    }
}
