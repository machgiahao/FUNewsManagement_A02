using AutoMapper;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagement.Services.DTOs.SystemAccount;
using FUNewsManagementRazorPages.ViewModels.SystemAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagementRazorPages.ViewModels.Auth;
using FUNewsManagementRazorPages.ViewModels.Category;
using NewsManagementMVC.Models.ViewModels.Auth;
using NewsManagementMVC.Models.ViewModels.NewsArticle;

namespace FUNewsManagement.Services.Mappings
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<CreateAccountViewModel, RegisterDto>();
            CreateMap<AccountViewModel, AccountDto>().ReverseMap();
            CreateMap<LoginViewModel, LoginDto>().ReverseMap();
            CreateMap<RegisterViewModel, RegisterDto>().ReverseMap();

            CreateMap<NewsArticleViewModel, NewsArticle>();
            CreateMap<NewsArticle, NewsArticleViewModel>()
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.AccountName : null));

            CreateMap<CreateNewsArticleViewModel, NewsArticle>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
            CreateMap<EditNewsArticleViewModel, NewsArticle>().ReverseMap();

            CreateMap<CategoryViewModel, Category>().ReverseMap();
        }
    }
}
