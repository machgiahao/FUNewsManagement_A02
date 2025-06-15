﻿
using FUNewsManagementSystem.BusinessObject.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NewsManagementMVC.Attributes
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public CustomAuthorizeAttribute(params AccountRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles.Select(r => (int)r).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetInt32("Role");
            if(role == null)
            {
                context.Result = new RedirectToPageResult("/Auth/Login", "Login", null);
                return;
            }
            if(!_allowedRoles.Contains(role.Value))
            {
                context.Result = new RedirectToPageResult("/Auth/AccessDenied");
            }

        }
    }
}
