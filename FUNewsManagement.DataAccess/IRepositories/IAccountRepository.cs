
using FUNewsManagement.BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public interface IAccountRepository
    {
        Task<SystemAccount> GetAccountByIdAsync(int id);
        Task<SystemAccount> GetAccountByEmailAsync(string email);
        Task CreateSystemAccountAsync(SystemAccount systemAccount);
        Task UpdateSystemAccountAsync(SystemAccount systemAccount);
        Task DeleteSystemAccountAsync(int id);
        Task<List<SystemAccount>> GetAllAccountsAsync();
        Task<bool> IsEmailExistedAsync(string email, int currentAccountId);
        Task<(List<SystemAccount> Accounts, int TotalCount)> GetAccountsPagedAsync(int page, int pageSize);

    }
}
