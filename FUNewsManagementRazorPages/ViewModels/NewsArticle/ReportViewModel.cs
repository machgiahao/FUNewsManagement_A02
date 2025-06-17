using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementRazorPages.ViewModels.NewsArticle
{
    public class ReportViewModel
    {
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
    }
}
