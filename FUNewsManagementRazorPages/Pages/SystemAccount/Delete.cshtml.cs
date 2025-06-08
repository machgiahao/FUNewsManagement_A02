using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagement.Services.DTOs.SystemAccount;
using FUNewsManagementSystem.Services;

namespace FUNewsManagementRazorPages.Pages.SystemAccount
{
    public class DeleteModel : PageModel
    {
        private readonly IAccountService _accountService;

        public DeleteModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public AccountDto SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemaccount = await _accountService.GetAccountById((int)id);

            if (systemaccount == null)
            {
                return NotFound();
            }
            else
            {
                SystemAccount = systemaccount;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemaccount = await _accountService.GetAccountById((int)id);
            if (systemaccount != null)
            {
                SystemAccount = systemaccount;
                await _accountService.DeleteSystemAccount((int)id);
            }

            return RedirectToPage("./Index");
        }
    }
}
