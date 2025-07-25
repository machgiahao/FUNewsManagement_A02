﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FUNewsManagementRazorPages.Pages.NewsArticles
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private readonly IHubContext<SignalRServer> _hubContext;
        public IndexModel(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService, IMapper mapper, IHubContext<SignalRServer> hubContext)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
            _mapper = mapper;
            _hubContext = hubContext;   
        }

        public IList<NewsArticleViewModel> NewsArticle { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public async Task OnGetAsync(int pageNumber = 1)
        {
            int pageSize = 6;
            var role = HttpContext.Session.GetInt32("Role");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            bool isStaff = role == (int)AccountRole.Staff;
            var (articles, totalCount) = await _newsArticleService.GetPagedNewsArticlesAsync(pageNumber, pageSize, isStaff);

            IEnumerable<NewsArticle> filtered = articles;

            NewsArticle = _mapper.Map<List<NewsArticleViewModel>>(filtered);

            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["CurrentPage"] = CurrentPage;
            ViewData["TotalPages"] = TotalPages;
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "CategoryName");
            ViewData["TagIds"] = new MultiSelectList(await _tagService.GetTagsAsync(), "TagId", "TagName");
        }

        [BindProperty]
        public CreateNewsArticleViewModel CreateNewsArticle { get; set; }
        
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!IsStaff())
                return RedirectToPage("/Auth/AccessDenied");
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "CategoryName");
            ViewData["TagIds"] = new MultiSelectList(await _tagService.GetTagsAsync(), "TagId", "TagName");

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
                var tags = await _tagService.GetTagsAsync();
                newsArticle.Tags = tags.Where(t => CreateNewsArticle.SelectedTagIds.Contains(t.TagId)).ToList();
            }

            newsArticle.CreatedDate = DateTime.UtcNow;
            var userId = HttpContext.Session.GetInt32("UserId");
            newsArticle.CreatedById = (short)userId.Value;
            await _newsArticleService.SaveNewsArticleAsync(newsArticle);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
            TempData["SuccessMessage"] = "Created successfully!"; 
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
            var article = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            // Create view model from entity
            EditNewsArticle = _mapper.Map<EditNewsArticleViewModel>(article);

            // Make sure SelectedTagIds is initialized and populated
            EditNewsArticle.SelectedTagIds = article.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();

            // Load category dropdown options
            ViewData["CategoryId"] = new SelectList(
                await _categoryService.GetCategoriesAsync(),
                "CategoryId",
                "CategoryName",
                article.CategoryId
            );

            // Load tag options with selected values
            ViewData["TagIds"] = new MultiSelectList(
                await _tagService.GetTagsAsync(),
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
                    await _categoryService.GetCategoriesAsync(),
                    "CategoryId",
                    "CategoryName"
                );
                ViewData["TagIds"] = new MultiSelectList(
                    await _tagService.GetTagsAsync(),
                    "TagId",
                    "TagName",
                    EditNewsArticle.SelectedTagIds
                );
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var article = await _newsArticleService.GetNewsArticleByIdAsync(EditNewsArticle.NewsArticleId);
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
                var tags = await _tagService.GetTagsAsync();
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

            await _newsArticleService.UpdateNewsArticleAsync(article);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
            TempData["SuccessMessage"] = "Updated successfully!";
            return RedirectToPage("./Index");
        }

        [BindProperty]
        public int DeleteAccountId { get; set; }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (!IsStaff())
                return RedirectToPage("/Auth/AccessDenied");
            await _newsArticleService.DeleteNewsArticleAsync(id);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
            TempData["SuccessMessage"] = "Deleted successfully!";
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnGetPartialAsync(int pageNumber = 1)
        {
            int pageSize = 6;
            var role = HttpContext.Session.GetInt32("Role");
            bool isStaff = role == (int)AccountRole.Staff;
            var (articles, totalCount) = await _newsArticleService.GetPagedNewsArticlesAsync(pageNumber, pageSize, isStaff);

            var mapped = _mapper.Map<List<NewsArticleViewModel>>(articles);

            var viewData = new ViewDataDictionary<List<NewsArticleViewModel>>(ViewData, mapped)
            {
                ["CurrentPage"] = pageNumber,
                ["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize),
                ["Role"] = role
            };

            return new PartialViewResult
            {
                ViewName = "_ListNewsArticlePartial",
                ViewData = viewData
            };
        }

        private bool IsStaff()
        {
            return HttpContext.Session.GetInt32("Role") == (int)AccountRole.Staff;
        }
    }
}

