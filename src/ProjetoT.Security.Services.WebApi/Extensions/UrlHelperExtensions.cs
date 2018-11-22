using System;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoT.Security.Services.WebApi.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: "ConfirmEmail",
                controller: "ConfirmEmail",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: "ResetPassword",
                controller: "ResetPassword",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
