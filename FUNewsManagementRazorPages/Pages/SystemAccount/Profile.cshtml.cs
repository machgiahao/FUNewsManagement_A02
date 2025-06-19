using AutoMapper;
using FUNewsManagementRazorPages.ViewModels.SystemAccount;
using FUNewsManagementSystem.BusinessObject.Enums;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementMVC.Attributes;

namespace FUNewsManagementRazorPages.Pages.SystemAccount;
[CustomAuthorize(AccountRole.Staff)]
public class ProfileModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    public ProfileModel(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }

    public AccountViewModel SystemAccount { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if( userId == null)
        {
            return RedirectToPage("/Auth/Login");
        }
        var account = await _accountService.GetAccountByIdAsync((int)userId);
        if (account == null)
        {
            return NotFound();
        }
        else
        {
            SystemAccount = _mapper.Map<AccountViewModel>(account);
        }
        return Page();
    }
}