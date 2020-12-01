using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chino.IdentityServer.Dtos.Account;
using Chino.IdentityServer.Extensions.Oidc;
using Chino.IdentityServer.Models.User;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Nekonya;

namespace Chino.IdentityServer.Pages.Account
{
    public class LoginModel : PageModel
    {
        /// <summary>
        /// 授权服务器交互服务
        /// </summary>
        private readonly IIdentityServerInteractionService m_IdsInteraction;
        private readonly SignInManager<ChinoUser> m_SignInManager;
        private readonly UserManager<ChinoUser> m_UserManager;
        private readonly IEventService m_IdsEvent;
        private readonly IStringLocalizer<LoginModel> L;

        public LoginModel(IIdentityServerInteractionService identityServerInteractionService,
            SignInManager<ChinoUser> signInManager,
            UserManager<ChinoUser> userManager,
            IEventService idsEvent,
            IStringLocalizer<LoginModel> localizer)
        {
            m_IdsInteraction = identityServerInteractionService;
            m_SignInManager = signInManager;
            m_UserManager = userManager;
            m_IdsEvent = idsEvent;
            L = localizer;
        }


        [BindProperty]
        public LoginDto LoginDto { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public bool EnableLocalLogin { get; set; } = true;

        public bool AllowRememberLogin { get; set; } = true;

        public IActionResult OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            var context = await m_IdsInteraction.GetAuthorizationContextAsync(ReturnUrl);
            if(button != "login")
            {
                if(context != null)
                {
                    /*
                     * 如果用户点击取消，则向IdentityServer发送一个结果（类似于用户拒绝同意）】
                     * 然后Oidc错误会被发回给客户端
                     */
                    await m_IdsInteraction.DenyAuthorizationAsync(context, IdentityServer4.Models.AuthorizationError.AccessDenied);

                    if (context.IsNativeClient())
                    {
                        //The client is native, so this change in how to return the response is for better UX for the end user.
                        //客户端是本地客户端，因此此更改返回响应的方式是为最终用户提供更好的UX。
                        //TODO : 这边跳转往后有时间再写
                    }

                    return Redirect("/Index");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await m_SignInManager.PasswordSignInAsync(LoginDto.Username, LoginDto.Password, LoginDto.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await m_UserManager.FindByNameAsync(LoginDto.Username);
                    await m_IdsEvent.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    if(context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            //The client is native, so this change in how to return the response is for better UX for the end user.
                            //客户端是本地客户端，因此此更改返回响应的方式是为最终用户提供更好的UX。
                            //TODO : 这边跳转往后有时间再写
                        }

                        return Redirect(ReturnUrl);
                    }

                    //请求来自本地页面
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else if (ReturnUrl.IsNullOrEmpty())
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception(L["invalid_return_URL", ReturnUrl]);
                    }
                }

                await m_IdsEvent.RaiseAsync(new UserLoginFailureEvent(LoginDto.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, L["invalid_credentials_error_message"]);
            }


            return Page();
        }
    }
}
