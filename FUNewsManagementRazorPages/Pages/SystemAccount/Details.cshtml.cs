using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagementSystem.Services;
using AutoMapper;
using FUNewsManagementRazorPages.ViewModels.SystemAccount;
using Microsoft.AspNetCore.SignalR;
using FUNewsManagementRazorPages.SignalR;

namespace FUNewsManagementRazorPages.Pages.SystemAccount
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public DetailsModel(IAccountService accountService, IMapper mapper, IHubContext<SignalRServer> hubContext)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        public AccountViewModel SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemaccount = await _accountService.GetAccountByIdAsync((int)id);
            if (systemaccount == null)
            {
                return NotFound();
            }
            else
            {
                SystemAccount = _mapper.Map<AccountViewModel>(systemaccount);
            }
            return Page();
        }
    }
}
