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
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IList<AccountDto> systemAccount { get;set; } = default!;

        public async Task OnGetAsync()
        {
            systemAccount = await _accountService.GetAllAccountsAsync();
        }
    }
}
