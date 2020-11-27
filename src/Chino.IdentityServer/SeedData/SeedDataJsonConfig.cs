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
        public Client[] Clients { get; set; }

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

        public class Client
        {
            public string ClientId { get; set; }
            public string ClientName { get; set; }
            public ICollection<string> AllowedGrantTypes { get; set; } = new HashSet<string>();
            public ICollection<string> ClientSecrets { get; set; } = new HashSet<string>();
            public ICollection<string> AllowedScopes { get; set; } = new HashSet<string>();

            public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();
            public string FrontChannelLogoutUri { get; set; }
            public ICollection<string> PostLogoutRedirectUris { get; set; } = new HashSet<string>();
            public bool AllowOfflineAccess { get; set; } = false;

            public string Description { get; set; }
        }
    }
}
