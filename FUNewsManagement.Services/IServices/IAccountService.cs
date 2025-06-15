using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagement.Services.DTOs.SystemAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface IAccountService
    {
        Task<AccountDto?> Login(LoginDto loginDto);
        Task DeleteSystemAccount(int newsArticleId);
        Task<List<AccountDto>> GetAllAccountsAsync();
        Task<(List<AccountDto> Accounts, int TotalCount)> GetAccountsPagedAsync(int page, int pageSize);
        Task CreateSystemAccount(RegisterDto registerDto);
        Task UpdateSystemAccount(AccountDto accountDto);
        Task<AccountDto?> GetAccountByIdAsync(int id);
        Task<List<AccountDto>> SearchAccounts(string searchField, string searchString);
        Task Register(RegisterDto registerDto);
        Task<AccountDto?> GetCurrentAccount(int id);
        Task<List<string>> GetAllAccountEmails();
        Task<bool> IsEmailExisted(string email, int currentAccountId);
        Task<bool> HasAccountChanged(string newName, string newEmail, SystemAccount existing);
        Task<ChangePasswordResult> ChangePassword(ChangePasswordDto changePasswordDto);
    }
}
