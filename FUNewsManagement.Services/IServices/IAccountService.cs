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
        Task CreateSystemAccount(RegisterDto registerDto);
        Task UpdateSystemAccount(AccountDto accountDto);
        Task<AccountDto?> GetAccountById(int id);
        Task<List<AccountDto>> SearchAccounts(string searchField, string searchString);
        Task Register(string name, string email, string password);
        Task<AccountDto?> GetCurrentAccount(int id);
        Task<List<string>> GetAllAccountEmails();
        Task<bool> IsEmailExisted(string email, int currentAccountId);
        Task<bool> HasAccountChanged(string newName, string newEmail, SystemAccount existing);
        Task<ChangePasswordResult> ChangePassword(ChangePasswordDto changePasswordDto);
    }
}
