using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino.IdentityServer.Models.User;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chino.IdentityServer.Pages.Account
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ChinoUser> m_SignInManager;
        private readonly IEventService m_IdsEvent;


        public LogoutModel(SignInManager<ChinoUser> signInManager,
            IEventService idsEvent)
        {
            m_SignInManager = signInManager;
            m_IdsEvent = idsEvent;
        }

        [BindProperty]
        public string LogoutId { get; set; }

        public void OnGet(string logoutId)
        {
            
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                await m_SignInManager.SignOutAsync();

                await m_IdsEvent.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }


            return Redirect("/");
        }
    }
}
