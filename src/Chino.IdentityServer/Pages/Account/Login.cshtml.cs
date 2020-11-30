using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chino.IdentityServer.Pages.Account
{
    public class LoginModel : PageModel
    {
        /// <summary>
        /// ∆Ù”√±æµÿ’À∫≈µ«¬º
        /// </summary>
        public bool EnableLocalLogin { get; set; } = true;


        public void OnGet()
        {
        }
    }
}
