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
using FUNewsManagement.Services.DTOs.SystemAccount;

namespace FUNewsManagementRazorPages.Pages.SystemAccount
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;

        public DetailsModel(IAccountService _accountService)
        {
            _accountService = _accountService;
        }

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
    }
}
