using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementSystem.Services;
using NewsManagementMVC.Models.ViewModels.NewsArticle;
using AutoMapper;

namespace FUNewsManagementRazorPages.Pages.NewsArticles
{
    public class DetailsModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly IMapper _mapper;

        public DetailsModel(INewsArticleService newsArticleService, IMapper mapper)
        {
            _newsArticleService = newsArticleService;
            _mapper = mapper;
        }

        public NewsArticleViewModel NewsArticle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            NewsArticle = _mapper.Map<NewsArticleViewModel>(newsArticle);
            return Page();
        }
    }
}
