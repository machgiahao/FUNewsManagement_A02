using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagementRazorPages.ViewModels.Auth;
using FUNewsManagement.Services.DTOs.SystemAccount;
using FUNewsManagementSystem.Services;
using AutoMapper;
using NewsManagementMVC.Models.ViewModels.Auth;

namespace FUNewsManagementRazorPages.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public RegisterModel(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            try
            {
                var registerDto = _mapper.Map<RegisterDto>(RegisterViewModel);
                await _accountService.Register(registerDto);
                TempData["RegisterSuccess"] = true;
                return RedirectToPage("Login");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("REGISTER_FAIL", $"Error in adding account: {e.Message}");
                return Page();
            }
        }
    }
}