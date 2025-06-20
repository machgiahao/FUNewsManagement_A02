using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementRazorPages.ViewModels.Category
{
    public class SearchCategoryViewModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchDescription { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchStatus { get; set; }
    }
}
