using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementRazorPages.ViewModels.Category;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsManagementMVC.Models.ViewModels.NewsArticle;
using NewsManagementMVC.Attributes;
using FUNewsManagementSystem.BusinessObject.Enums;

namespace FUNewsManagementRazorPages.Pages.Categories
{
    [CustomAuthorize(AccountRole.Staff)]
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public IndexModel(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public IList<CategoryViewModel> CategoryViewModel { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public SearchCategoryViewModel Search { get; set; } = new();

        public async Task OnGetAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();

            // Filter by name
            if (!string.IsNullOrWhiteSpace(Search.SearchName))
                categories = categories.Where(c => c.CategoryName.Contains(Search.SearchName, StringComparison.OrdinalIgnoreCase)).ToList();

            // Filter by description
            if (!string.IsNullOrWhiteSpace(Search.SearchDescription))
                categories = categories.Where(c => c.CategoryDesciption != null && c.CategoryDesciption.Contains(Search.SearchDescription, StringComparison.OrdinalIgnoreCase)).ToList();

            // Filter by status
            if (!string.IsNullOrWhiteSpace(Search.SearchStatus) && (Search.SearchStatus == "Active" || Search.SearchStatus == "Inactive"))
            {
                bool isActive = Search.SearchStatus == "Active";
                categories = categories.Where(c => c.IsActive == isActive).ToList();
            }

            // Map to ViewModel
            var viewModels = _mapper.Map<IList<CategoryViewModel>>(categories);

            // Parent name mapping
            var categoryDict = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
            foreach (var item in viewModels)
            {
                if (item.ParentCategoryId.HasValue && categoryDict.TryGetValue((short)item.ParentCategoryId.Value, out var parentName))
                    item.ParentCategoryName = parentName;
                else
                    item.ParentCategoryName = "";
            }

            CategoryViewModel = viewModels;

            // Dropdown
            categories.Insert(0, new Category { CategoryId = 0, CategoryName = "-- Select Parent Category --" });
            ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
        }



        [BindProperty]
        public CategoryViewModel CreateCategoryViewModel { get; set; } = new();
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var createCategory = _mapper.Map<Category>(CreateCategoryViewModel);
            await _categoryService.CreateCategoryAsync(createCategory);
            TempData["SuccessMessage"] = "Created successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            UpdateCategoryViewModel = _mapper.Map<CategoryViewModel>(category);
            await OnGetAsync();

            ViewData["ShowEditModal"] = true;
            return Page();
        }

        [BindProperty]
        public CategoryViewModel UpdateCategoryViewModel { get; set; } = new();
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }
            var updateCategory = _mapper.Map<Category>(UpdateCategoryViewModel);
            await _categoryService.UpdateCategoryAsync(updateCategory);
            TempData["SuccessMessage"] = "Updated successfully!";
            return RedirectToPage();
        }

        [BindProperty]
        public int DeleteCategoryId { get; set; }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Delete successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Set a user-friendly error message
                TempData["DeleteError"] = "Cannot delete this category because it is already in use";
                return RedirectToPage();
            }
        }
    }
}
