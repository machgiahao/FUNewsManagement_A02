using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FUNewsManagementSystem.BusinessObject.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FUNewsManagementSystem.BusinessObject;

namespace NewsManagementMVC.Models.ViewModels.NewsArticle
{
    public class EditNewsArticleViewModel
    {
        public string NewsArticleId { get; set; }
        [Required(ErrorMessage = "News title is required.")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        public string Headline { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool? NewsStatus { get; set; }

        public List<int> SelectedTagIds { get; set; } = new();

    }

}
