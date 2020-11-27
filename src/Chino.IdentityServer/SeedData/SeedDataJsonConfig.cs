using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chino.IdentityServer.SeedData
{
    /// <summary>
    /// Json -> Object 
    /// </summary>
    public class SeedDataJsonConfig
    {
        public User[] Users { get; set; }

        public class User
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public bool EmailConfirmed { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }

            public string Password { get; set; }

            #region Claims

            public string Name { get; set; }
            public string WebSite { get; set; }
            public string NickName { get; set; }

            #endregion
        }
    }
}
