using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chino.IdentityServer.Extensions.Oidc
{
    public static class OidcExtensions
    {
        /// <summary>
        /// Checks if the redirect URI is for a native client.
        /// 检查重定向URI判断是否是原生客户端
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsNativeClient(this AuthorizationRequest context)
        {
            return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }

        //public static IActionResult LoadingPage(this PageModel pageModel, string viewName, string redirectUri)
        //{
        //    pageModel.HttpContext.Response.StatusCode = 200;
        //    pageModel.HttpContext.Response.Headers["Location"] = "";

        //    pageModel.re
        //    return pageModel.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
        //}
    }
}
