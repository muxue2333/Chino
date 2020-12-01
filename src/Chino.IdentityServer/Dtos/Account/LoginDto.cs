using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chino.IdentityServer.Pages.Account;

namespace Chino.IdentityServer.Dtos.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "username_required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "passwd_required")]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }
    }
}
