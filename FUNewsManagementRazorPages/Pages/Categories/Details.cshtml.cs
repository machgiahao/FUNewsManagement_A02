using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using AutoMapper;
using FUNewsManagementSystem.Services;
using FUNewsManagementRazorPages.ViewModels.Category;

namespace FUNewsManagementRazorPages.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public DetailsModel(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public CategoryViewModel Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if(category == null)
            {
                return NotFound();
            } 
            var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
            if(categoryViewModel.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryService.GetCategoryByIdAsync(categoryViewModel.ParentCategoryId.Value);
                if (parentCategory != null)
                {
                    categoryViewModel.ParentCategoryName = parentCategory.CategoryName;
                }
                else
                {
                    categoryViewModel.ParentCategoryName = "--";
                }
            }
            Category = categoryViewModel;
            return Page();
        }
    }
}
