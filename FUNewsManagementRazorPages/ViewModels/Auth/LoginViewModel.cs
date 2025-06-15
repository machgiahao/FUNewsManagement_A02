﻿using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementRazorPages.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email cannot be blank !")]
        [EmailAddress(ErrorMessage = "Email is incorrect format !")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "Password cannot be blank !")]
        public string AccountPassword { get; set; }
    }
}
