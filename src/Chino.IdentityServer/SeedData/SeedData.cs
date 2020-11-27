using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino.IdentityServer.Data;
using Chino.IdentityServer.Models.User;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Text;
using AutoMapper;
using Chino.AutoMapper;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.Mappers;

namespace Chino.IdentityServer.SeedData
{
    public class SeedData
    {
        public static void EnsureSeedData(IConfiguration configuration)
        {
            SeedDataJsonConfig seedDataConfig;
            try
            {
                string seedDataJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "seedData.json"), Encoding.UTF8);
                seedDataConfig = JsonSerializer.Deserialize<SeedDataJsonConfig>(seedDataJson);
            }
            catch(Exception e)
            {
                Console.WriteLine("[Read SeedData From Json File Failed!] " + e.Message);
                seedDataConfig = new SeedDataJsonConfig();
            }

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddChinoDatabase<ChinoApplicationDbContext>(configuration);
            services.AddIdentity<ChinoUser, IdentityRole>()
                .AddEntityFrameworkStores<ChinoApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.AddIdentityServerOperationalDatabase(configuration);
            });
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.AddIdentityServerConfigurationDatabase(configuration);
            });

            services.AddAutoMapper(typeof(AutoMapperProfile));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var chinoAppContext = scope.ServiceProvider.GetService<ChinoApplicationDbContext>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ChinoUser>>();
            var mapper = scope.ServiceProvider.GetService<IMapper>();
            EnsureChinoAppSeedData(ref chinoAppContext, ref userMgr, ref seedDataConfig, ref mapper);

            var idsConfigurationContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            var idsPersistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();

            EnsureIdentityServerSeedData(ref idsConfigurationContext, ref idsPersistedGrantDbContext, ref seedDataConfig);
        }

        private static void EnsureChinoAppSeedData(ref ChinoApplicationDbContext context, ref UserManager<ChinoUser> userMgr , ref SeedDataJsonConfig seedDataJson, ref IMapper mapper)
        {
            context.Database.Migrate();

            #region Users Seed
            if (seedDataJson.Users == null || seedDataJson.Users.Length == 0)
            {
                seedDataJson.Users = new SeedDataJsonConfig.User[]
                {
                    new SeedDataJsonConfig.User
                    {
                        UserName = "alice",
                        Email = "alice@corala.space",
                        EmailConfirmed = true,
                        Password = "Abc123456!",
                        Name = "Kirito Alice",
                        NickName = "Alice",
                        WebSite = "https://alice.moe"
                    }
                };
            }

            foreach(var _user in seedDataJson.Users)
            {
                var userInfo = userMgr.FindByNameAsync(_user.UserName).Result;
                if (userInfo == null)
                {
                    userInfo = mapper.Map<ChinoUser>(_user);
                    var result = userMgr.CreateAsync(userInfo, _user.Password).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(userInfo, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, _user.Name),
                        new Claim(JwtClaimTypes.NickName, _user.NickName),
                        new Claim(JwtClaimTypes.WebSite, _user.WebSite),
                    }).Result;
                }
            }


            #endregion
        }

        private static void EnsureIdentityServerSeedData(ref ConfigurationDbContext configurationContext, ref PersistedGrantDbContext persistedGrantContext, ref SeedDataJsonConfig seedDataJson)
        {
            configurationContext.Database.Migrate();
            persistedGrantContext.Database.Migrate();

            if (!configurationContext.Clients.Any())
            {
                if(seedDataJson.Clients == null || seedDataJson.Clients.Length == 0)
                {
                    //var client1 = new SeedDataJsonConfig.Client();
                    //client1.ClientId = "m2m.client";
                    //client1.ClientName = "Client Credentials Client";

                    //client1.AllowedGrantTypes = GrantTypes.ClientCredentials;

                    //client1.ClientSecrets = { "511536EF-F270-4058-80CA-1C89C192F69A" };
                    //client1.AllowedScopes = { "scope1" };
                    seedDataJson.Clients = new SeedDataJsonConfig.Client[]
                    {
                        new SeedDataJsonConfig.Client
                        {
                            ClientId = "m2m.client",
                            ClientName = "Client Credentials Client",

                            AllowedGrantTypes = GrantTypes.ClientCredentials,
                            ClientSecrets = { "511536EF-F270-4058-80CA-1C89C192F69A" },

                            AllowedScopes = { "scope1" }
                        },
                        new SeedDataJsonConfig.Client
                        {
                            ClientId = "interactive",
                            ClientSecrets = new string[]{ "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0" },

                            AllowedGrantTypes = GrantTypes.Code,

                            RedirectUris = { "https://localhost:44300/signin-oidc" },
                            FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                            PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                            AllowOfflineAccess = true,
                            AllowedScopes = { "openid", "profile", "scope2" }
                        }
                    };
                }
                foreach(var _client in seedDataJson.Clients)
                {
                    var idsClient = new Client
                    {
                        ClientId = _client.ClientId,
                        ClientName = _client.ClientName,
                        AllowedGrantTypes = _client.AllowedGrantTypes,
                        ClientSecrets = _client.ClientSecrets.Select(str => new Secret(str.Sha256())).ToArray(),
                        AllowedScopes = _client.AllowedScopes,
                        RedirectUris = _client.RedirectUris,
                        FrontChannelLogoutUri = _client.FrontChannelLogoutUri,
                        PostLogoutRedirectUris = _client.PostLogoutRedirectUris,
                        AllowOfflineAccess = _client.AllowOfflineAccess,
                        Description = _client.Description
                    };

                    configurationContext.Clients.Add(idsClient.ToEntity());
                }
                configurationContext.SaveChanges();
            }
        }

    }
}
