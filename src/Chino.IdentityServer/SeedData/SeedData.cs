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

    }
}
