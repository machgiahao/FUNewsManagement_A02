using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementSystem.Services;
using FUNewsManagement.Services.DTOs.SystemAccount;

namespace FUNewsManagementRazorPages.Pages.SystemAccount
{
    public class CreateModel : PageModel
    {
        private readonly IAccountService _accountService;

        public CreateModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RegisterDto systemAccount { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _accountService.CreateSystemAccount(systemAccount);        

            return RedirectToPage("./Index");
        }
    }
}
