using AutoMapper;
using FUNewsManagementRazorPages.SignalR;
using FUNewsManagementSystem.BusinessObject.Enums;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NewsManagementMVC.Attributes;
using NewsManagementMVC.Models.ViewModels.NewsArticle;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUNewsManagementRazorPages.Pages.NewsArticles;
[CustomAuthorize(AccountRole.Staff)]
public class MyNewsArticleModel : PageModel
{
    private readonly INewsArticleService _newsArticleService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly IMapper _mapper;
    private readonly IHubContext<SignalRServer> _hubContext;

    public MyNewsArticleModel(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService, IMapper mapper, IHubContext<SignalRServer> hubContext)
    {
        _newsArticleService = newsArticleService;
        _categoryService = categoryService;
        _tagService = tagService;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public NewsArticleViewModel NewsArticle { get; set; } = default!;
    public List<NewsArticleViewModel> MyNewsArticles { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Auth/Login");
        }
        var newsArticle = await _newsArticleService.GetListNewsArticlesByCreatorAsync((int)userId);
        MyNewsArticles = newsArticle != null ? _mapper.Map<List<NewsArticleViewModel>>(newsArticle) : new List<NewsArticleViewModel>();
        ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "CategoryName");
        ViewData["TagIds"] = new MultiSelectList(await _tagService.GetTagsAsync(), "TagId", "TagName");
        return Page();
    }
}