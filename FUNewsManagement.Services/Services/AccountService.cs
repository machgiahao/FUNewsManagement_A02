using AutoMapper;
using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagement.Services.DTOs.SystemAccount;
using FUNewsManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _iAccountRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository iAccountRepository, IConfiguration config, IMapper mapper)
        {
            _iAccountRepository = iAccountRepository;
            _config = config;
            _mapper = mapper;
        }

        public async Task<AccountDto?> Login(LoginDto loginDto)
        {
            var adminEmail = _config["AdminAccount:Email"];
            var adminPassword = _config["AdminAccount:Password"];

            if (loginDto.AccountEmail == adminEmail && loginDto.AccountPassword == adminPassword)
            {
                return new AccountDto
                {
                    AccountId = 0,
                    AccountName = "Admin",
                    AccountEmail = loginDto.AccountEmail,
                    AccountRole = 0
                };
            }

            var user = await _iAccountRepository.GetAccountByEmailAsync(loginDto.AccountEmail);
            if (user != null && user.AccountPassword == loginDto.AccountPassword)
            {
                return _mapper.Map<AccountDto>(user);
            }

            return null;
        }

        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _iAccountRepository.GetAllAccountsAsync();
            return _mapper.Map<List<AccountDto>>(accounts);
        }

        public async Task<(List<AccountDto> Accounts, int TotalCount)> GetAccountsPagedAsync(int page, int pageSize)
        {
            var (accounts, totalCount) = await _iAccountRepository.GetAccountsPagedAsync(page, pageSize);
            var accountDtos = _mapper.Map<List<AccountDto>>(accounts);
            return (accountDtos, totalCount);
        }

        public async Task DeleteSystemAccount(int newsArticleId)
        {
            await _iAccountRepository.DeleteSystemAccountAsync(newsArticleId);
        }

        public async Task CreateSystemAccount(RegisterDto registerDto)
        {
            var account = _mapper.Map<SystemAccount>(registerDto);
            await _iAccountRepository.CreateSystemAccountAsync(account);
        }

        public async Task UpdateSystemAccount(AccountDto accountDto)
        {
            var account = _mapper.Map<SystemAccount>(accountDto);
            await _iAccountRepository.UpdateSystemAccountAsync(account);
        }

        public async Task<AccountDto?> GetAccountByIdAsync(int id)
        {
            var account = await _iAccountRepository.GetAccountByIdAsync(id);
            return account != null ? _mapper.Map<AccountDto>(account) : null;
        }

        public async Task<List<AccountDto>> SearchAccounts(string searchField, string searchString)
        {
            var accounts = await _iAccountRepository.GetAllAccountsAsync();
            IEnumerable<SystemAccount> filtered = accounts;

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchField)
                {
                    case "AccountEmail":
                        filtered = accounts.Where(a => a.AccountEmail != null && a.AccountEmail.Contains(searchString, System.StringComparison.OrdinalIgnoreCase));
                        break;
                    case "AccountRole":
                        filtered = accounts.Where(a => a.AccountRole.ToString().Contains(searchString, System.StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        filtered = accounts.Where(a => a.AccountName != null && a.AccountName.Contains(searchString, System.StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            return _mapper.Map<List<AccountDto>>(filtered.ToList());
        }

        public async Task Register(RegisterDto registerDto)
        {
            var existing = await _iAccountRepository.GetAccountByEmailAsync(registerDto.AccountEmail);
            if (existing != null)
                throw new System.Exception("Email already exist");

            var account = new SystemAccount
            {
                AccountName = registerDto.AccountName,
                AccountEmail = registerDto.AccountEmail,
                AccountPassword = registerDto.AccountPassword,
                AccountRole = 2
            };
            await _iAccountRepository.CreateSystemAccountAsync(account);
        }

        public async Task<AccountDto?> GetCurrentAccount(int id)
        {
            var account = await _iAccountRepository.GetAccountByIdAsync(id);
            return account != null ? _mapper.Map<AccountDto>(account) : null;
        }

        public async Task<List<string>> GetAllAccountEmails()
        {
            var accounts = await _iAccountRepository.GetAllAccountsAsync();
            return accounts
                .Where(a => !string.IsNullOrWhiteSpace(a.AccountEmail))
                .Select(a => a.AccountEmail.ToLower())
                .ToList();
        }

        public async Task<bool> IsEmailExisted(string email, int currentAccountId)
        {
            return await _iAccountRepository.IsEmailExistedAsync(email, currentAccountId);
        }

        public async Task<bool> HasAccountChanged(string newName, string newEmail, SystemAccount existing)
        {
            return !string.Equals(newName?.Trim(), existing.AccountName, System.StringComparison.Ordinal) ||
                   !string.Equals(newEmail?.Trim(), existing.AccountEmail, System.StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ChangePasswordResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = new ChangePasswordResult();

            var account = await _iAccountRepository.GetAccountByIdAsync(changePasswordDto.UserId);
            if (account == null)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Account not found.";
                return result;
            }

            if (!account.AccountPassword.Equals(changePasswordDto.CurrentPassword))
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Current password is incorrect.";
                return result;
            }

            account.AccountPassword = changePasswordDto.NewPassword;

            try
            {
                await _iAccountRepository.UpdateSystemAccountAsync(account);
                result.IsSuccess = true;
                result.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Error changing password: " + ex.Message;
            }

            return result;
        }
    }

}
