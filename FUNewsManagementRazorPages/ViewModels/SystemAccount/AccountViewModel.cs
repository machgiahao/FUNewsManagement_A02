using System.ComponentModel.DataAnnotations;
using FUNewsManagementSystem.BusinessObject;
using FUNewsManagementSystem.Services;

namespace FUNewsManagementRazorPages.ViewModels.SystemAccount
{
    public class AccountViewModel
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be longer than 2 characters.")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be valid.")]
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var _accountService = (IAccountService)validationContext.GetService(typeof(IAccountService));

        //    if (_accountService != null && !string.IsNullOrWhiteSpace(AccountEmail))
        //    {
        //        var currentAccount = _accountService.GetCurrentAccount(AccountId);
        //        var allOtherEmails = _accountService.GetAllAccountEmails()
        //            .Where(e => !e.Equals(currentAccount?.AccountEmail, StringComparison.OrdinalIgnoreCase))
        //            .ToList();

        //        if (allOtherEmails.Contains(AccountEmail.Trim(), StringComparer.OrdinalIgnoreCase))
        //        {
        //            yield return new ValidationResult("Email is already in use.", new[] { nameof(AccountEmail) });
        //        }
        //    }
        //}
    }
}
