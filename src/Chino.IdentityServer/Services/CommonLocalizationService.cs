using System.Reflection;
using Chino.IdentityServer.Resources.Common;
using Microsoft.Extensions.Localization;

namespace Chino.IdentityServer.Services
{
    public class CommonLocalizationService
    {
        private readonly IStringLocalizer localizer;
        public CommonLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(CommonResources).GetTypeInfo().Assembly.FullName);
            
            localizer = factory.Create($"Common.{nameof(CommonResources)}", assemblyName.Name);
        }

        public string this[string key]
        {
            get
            {
                return localizer[key];
            }
        }

        public string Get(string key)
        {
            return localizer[key];
        }

    }
}
