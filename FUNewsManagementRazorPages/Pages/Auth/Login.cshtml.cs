using AutoMapper;
using FUNewsManagement.Services.DTOs.SystemAccount;
using FUNewsManagementRazorPages.ViewModels.Auth;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementRazorPages.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        
        public LoginModel(IAccountService accountService, IConfiguration config, IMapper mapper)
        {
            _accountService = accountService;
            _config = config;
            _mapper = mapper;
        }
        
        public PageResult OnGet()
        {
            return Page();
        }
        
        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userdto = _mapper.Map<LoginDto>(LoginViewModel);
                var user = await _accountService.Login(userdto);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.AccountId);
                    HttpContext.Session.SetString("Username", user.AccountName);
                    HttpContext.Session.SetInt32("Role", user.AccountRole ?? -1);
                    if (user.AccountRole == 0)
                    {
                        return RedirectToPage("/SystemAccount/Index");
                    }
                    return RedirectToPage("/NewsArticles/Index");
                }
                else
                {
                    ModelState.AddModelError("LOGIN_FAIL", "Invalid username or password");
                }
            }

            return Page();
        }
    }
}
