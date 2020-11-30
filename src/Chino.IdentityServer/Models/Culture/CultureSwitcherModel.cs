using System.Collections.Generic;
using System.Globalization;

namespace Chino.IdentityServer.Models.Culture
{
    public class CultureSwitcherModel
    {
        public CultureInfo CurrentUICulture { get; set; }
        public List<CultureInfo> SupportedCultures { get; set; }
    }
}
