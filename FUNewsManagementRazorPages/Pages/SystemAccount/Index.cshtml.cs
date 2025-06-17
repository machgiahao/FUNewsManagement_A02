using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Services.DTOs.SystemAccount;
using FUNewsManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using FUNewsManagementRazorPages.ViewModels.SystemAccount;
using FUNewsManagementSystem.BusinessObject.Enums;
using NewsManagementMVC.Attributes;

namespace FUNewsManagementRazorPages.Pages.SystemAccount
{
    [CustomAuthorize(AccountRole.Admin)]
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public IndexModel(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        public IList<AccountViewModel> SystemAccount { get; set; } = new List<AccountViewModel>();
        [BindProperty(SupportsGet = true)]
        public SearchViewModel Search { get; set; } = new();


        public async Task OnGetAsync()
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            if (!string.IsNullOrWhiteSpace(Search.Name))
                accounts = accounts.Where(a => a.AccountName != null && a.AccountName.Contains(Search.Name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(Search.Email))
                accounts = accounts.Where(a => a.AccountEmail != null && a.AccountEmail.Contains(Search.Email, StringComparison.OrdinalIgnoreCase)).ToList();

            if (Search.Role.HasValue)
                accounts = accounts.Where(a => a.AccountRole == Search.Role).ToList();

            SystemAccount = _mapper.Map<List<AccountViewModel>>(accounts);
        }


        [BindProperty]
        public CreateAccountViewModel CreateAccountViewModel { get; set; } = new();
        public async Task<IActionResult> OnPostCreateAccountAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }
            var registerDto = _mapper.Map<RegisterDto>(CreateAccountViewModel);
            await _accountService.CreateSystemAccount(registerDto);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            EditAccountViewModel = _mapper.Map<AccountViewModel>(account);

            // Load the list for the table as well
            await OnGetAsync();

            ViewData["ShowEditModal"] = true;
            return Page();
        }

        [BindProperty]
        public AccountViewModel EditAccountViewModel { get; set; } = new();

        // Handle edit account POST
        public async Task<IActionResult> OnPostEdit()
        {
            if (ModelState.IsValid)
            {
                await OnGetAsync(); // Load all accounts again
                return Page();
            }
            ViewData["ShowEditModal"] = true;

            var accountDto = _mapper.Map<AccountDto>(EditAccountViewModel);
            await _accountService.UpdateSystemAccount(accountDto);

            return RedirectToPage("./Index");
        }

        // Add this to your IndexModel class

        [BindProperty]
        public int DeleteAccountId { get; set; }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _accountService.DeleteSystemAccount(id);
            return RedirectToPage("./Index");
        }

    }
}
