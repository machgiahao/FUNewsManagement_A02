using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagementSystem.Services;
using NewsManagementMVC.Models.ViewModels.NewsArticle;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementSystem.BusinessObject.Enums;
using NewsManagementMVC.Attributes;
using FUNewsManagementRazorPages.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace FUNewsManagementRazorPages.Pages.NewsArticles
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _contextNewsArticle;
        private readonly ICategoryService _contextCategory;
        private readonly ITagService _contextTag;
        private readonly IMapper _mapper;
        private readonly IHubContext<SignalRServer> _hubContext;
        public IndexModel(INewsArticleService contextNewsArticle, ICategoryService contextCategory, ITagService contextTag, IMapper mapper, IHubContext<SignalRServer> hubContext)
        {
            _contextNewsArticle = contextNewsArticle;
            _contextCategory = contextCategory;
            _contextTag = contextTag;
            _mapper = mapper;
            _hubContext = hubContext;   
        }

        public IList<NewsArticleViewModel> NewsArticle { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var role = HttpContext.Session.GetInt32("Role");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            var newsArticles = await _contextNewsArticle.GetNewsArticlesAsync();
            if (role == (int)AccountRole.Staff)
            {
                NewsArticle = _mapper.Map<List<NewsArticleViewModel>>(newsArticles);
            }
            else
            {
                var activeNews = newsArticles.Where(n => n.NewsStatus == true); 
                NewsArticle = _mapper.Map<List<NewsArticleViewModel>>(activeNews);
            }
            ViewData["CategoryId"] = new SelectList(await _contextCategory.GetCategoriesAsync(), "CategoryId", "CategoryName");
            ViewData["TagIds"] = new MultiSelectList(await _contextTag.GetTagsAsync(), "TagId", "TagName");
        }

        [BindProperty]
        public CreateNewsArticleViewModel CreateNewsArticle { get; set; }
        
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!IsStaff())
                return RedirectToPage("/Auth/AccessDenied");
            ViewData["CategoryId"] = new SelectList(await _contextCategory.GetCategoriesAsync(), "CategoryId", "CategoryName");
            ViewData["TagIds"] = new MultiSelectList(await _contextTag.GetTagsAsync(), "TagId", "TagName");

            if (ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Handle image upload
            if (CreateNewsArticle.ImageFile != null && CreateNewsArticle.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images", "NewsArticle");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(CreateNewsArticle.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CreateNewsArticle.ImageFile.CopyToAsync(stream);
                }
                // Save relative path for use in <img src="...">
                CreateNewsArticle.ImageUrl = $"/images/NewsArticle/{uniqueFileName}";
            }

            var newsArticle = _mapper.Map<NewsArticle>(CreateNewsArticle);
            newsArticle.NewsArticleId = Guid.NewGuid().ToString();

            if (CreateNewsArticle.SelectedTagIds != null && CreateNewsArticle.SelectedTagIds.Any())
            {
                var tags = await _contextTag.GetTagsAsync();
                newsArticle.Tags = tags.Where(t => CreateNewsArticle.SelectedTagIds.Contains(t.TagId)).ToList();
            }

            newsArticle.CreatedDate = DateTime.UtcNow;
            var userId = HttpContext.Session.GetInt32("UserId");
            newsArticle.CreatedById = (short)userId.Value;
            await _contextNewsArticle.SaveNewsArticleAsync(newsArticle);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetEditAsync(string id)
        {
            if (!IsStaff())
                return RedirectToPage("/Auth/AccessDenied");
            if (string.IsNullOrEmpty(id))
                return NotFound();
            await OnGetAsync();
            // Get the news article with its related tags
            var article = await _contextNewsArticle.GetNewsArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            // Create view model from entity (similar to your MVC FromNewsArticle)
            EditNewsArticle = _mapper.Map<EditNewsArticleViewModel>(article);

            // Make sure SelectedTagIds is initialized and populated
            EditNewsArticle.SelectedTagIds = article.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();

            // Load category dropdown options
            ViewData["CategoryId"] = new SelectList(
                await _contextCategory.GetCategoriesAsync(),
                "CategoryId",
                "CategoryName",  // Use CategoryName, not CategoryId for display
                article.CategoryId
            );

            // Load tag options with selected values
            ViewData["TagIds"] = new MultiSelectList(
                await _contextTag.GetTagsAsync(),
                "TagId",
                "TagName",
                EditNewsArticle.SelectedTagIds
            );

            ViewData["ShowEditModal"] = true;
            return Page();
        }


        [BindProperty]
        public EditNewsArticleViewModel EditNewsArticle { get; set; }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!IsStaff())
                return RedirectToPage("/Auth/AccessDenied");
            if (!ModelState.IsValid)
            {
                // If model state is invalid, reload necessary data and return to the page
                ViewData["CategoryId"] = new SelectList(
                    await _contextCategory.GetCategoriesAsync(),
                    "CategoryId",
                    "CategoryName"
                );
                ViewData["TagIds"] = new MultiSelectList(
                    await _contextTag.GetTagsAsync(),
                    "TagId",
                    "TagName",
                    EditNewsArticle.SelectedTagIds
                );
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var article = await _contextNewsArticle.GetNewsArticleByIdAsync(EditNewsArticle.NewsArticleId);
            if (article == null)
                return NotFound();

            // Map all updatable fields from the view model to the entity
            _mapper.Map(EditNewsArticle, article);

            // Set additional properties like in your ToNewsArticle method
            article.ModifiedDate = DateTime.UtcNow;
            var userId = HttpContext.Session.GetInt32("UserId");
            article.UpdatedById = (short)userId.Value;

            // Update tags
            if (EditNewsArticle.SelectedTagIds != null && EditNewsArticle.SelectedTagIds.Any())
            {
                var tags = await _contextTag.GetTagsAsync();
                article.Tags = tags.Where(t => EditNewsArticle.SelectedTagIds.Contains(t.TagId)).ToList();
            }
            else
            {
                // Clear all tags if none selected
                article.Tags?.Clear();
            }

            // Handle image upload if new image is provided
            if (EditNewsArticle.ImageFile != null && EditNewsArticle.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images", "NewsArticle");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(EditNewsArticle.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await EditNewsArticle.ImageFile.CopyToAsync(stream);
                }
                
                article.ImageUrl = $"/images/NewsArticle/{uniqueFileName}";
            }

            await _contextNewsArticle.UpdateNewsArticleAsync(article);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
            return RedirectToPage("./Index");
        }

        [BindProperty]
        public int DeleteAccountId { get; set; }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (!IsStaff())
                return RedirectToPage("/Auth/AccessDenied");
            await _contextNewsArticle.DeleteNewsArticleAsync(id);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
            return RedirectToPage("./Index");
        }
        
        private bool IsStaff()
        {
            return HttpContext.Session.GetInt32("Role") == (int)AccountRole.Staff;
        }
    }
}

