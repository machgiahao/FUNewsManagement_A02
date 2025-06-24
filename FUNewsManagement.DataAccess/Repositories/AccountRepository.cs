using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<SystemAccount?> GetAccountByEmailAsync(string email)
        {
            using var context = new FunewsManagementContext();
            return await context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail.ToLower() == email.ToLower());
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(int id)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.SystemAccounts
                    .FirstOrDefaultAsync(a => a.AccountId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in get account by ID: " + ex.Message);
            }
        }

        public async Task DeleteSystemAccountAsync(int accountId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var account = await context.SystemAccounts
                    .SingleOrDefaultAsync(a => a.AccountId == accountId);

                if (account != null)
                {
                    context.SystemAccounts.Remove(account);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in deleting account: " + ex.Message);
            }
        }

        public async Task<List<SystemAccount>> GetAllAccountsAsync()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.SystemAccounts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in get all accounts: " + ex.Message);
            }
        }

        public async Task<(List<SystemAccount> Accounts, int TotalCount)> GetAccountsPagedAsync(int page, int pageSize)
        {
            using var context = new FunewsManagementContext();
            var query = context.SystemAccounts.AsQueryable();

            int totalCount = await query.CountAsync();
            var accounts = await query
                .OrderBy(a => a.AccountId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (accounts, totalCount);
        }


        public async Task CreateSystemAccountAsync(SystemAccount systemAccount)
        {
            try
            {
                using var context = new FunewsManagementContext();

                if (systemAccount.AccountId == 0)
                {
                    systemAccount.AccountId = await GetNextAccountIdAsync(context);
                }

                await context.SystemAccounts.AddAsync(systemAccount);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in adding account: " + ex.Message);
            }
        }

        public async Task UpdateSystemAccountAsync(SystemAccount systemAccount)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<SystemAccount>(systemAccount).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in updating account: " + ex.Message);
            }
        }

        private async Task<short> GetNextAccountIdAsync(FunewsManagementContext context)
        {
            var maxId = await context.SystemAccounts.MaxAsync(a => (short?)a.AccountId) ?? 0;
            return (short)(maxId + 1);
        }

        public async Task<bool> IsEmailExistedAsync(string email, int currentAccountId)
        {
            using var context = new FunewsManagementContext();
            return await context.SystemAccounts.AnyAsync(a =>
                a.AccountEmail.ToLower() == email.ToLower() &&
                a.AccountId != currentAccountId);
        }
    }
}
