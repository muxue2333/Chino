using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chino.IdentityServer
{
    public static class ServicesExtension
    {
        public static void AddChinoDatabase<TAppDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TAppDbContext : DbContext
        {
            string connectionString = configuration.GetConnectionString("ChinoApp");
            string providerType = configuration["Database:ProviderType:Chino:App"] ?? "sqlite";
            switch (providerType.ToLower())
            {
                case "mysql":
                case "mariadb":
                    services.AddDbContext<TAppDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                    break;
                case "sqlite":
                case "sqlite3":
                    services.AddDbContext<TAppDbContext>(options =>
                        options.UseSqlite(connectionString));
                    break;
                case "sqlserver":
                case "mssql":
                    services.AddDbContext<TAppDbContext>(options =>
                        options.UseSqlServer(connectionString));
                    break;
                default:
                    throw new Exception($"Unknow database provider type: {providerType} - Chino Application");
            }

        }

        public static void AddIdentityServerConfigurationDatabase(this Microsoft.EntityFrameworkCore.DbContextOptionsBuilder builder, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("IdentityServerConfiguration");
            string providerType = configuration["Database:ProviderType:IdentityServer:Configuration"] ?? "sqlite";
            string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            switch (providerType.ToLower())
            {
                case "mysql":
                case "mariadb":
                    builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case "sqlite":
                case "sqlite3":
                    builder.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case "sqlserver":
                case "mssql":
                    builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                default:
                    throw new Exception($"Unknow database provider type: {providerType} - IdentityServer Configuration");
            }
        }

        public static void AddIdentityServerOperationalDatabase(this Microsoft.EntityFrameworkCore.DbContextOptionsBuilder builder, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("IdentityServerOperational");
            string providerType = configuration["Database:ProviderType:IdentityServer:Operational"] ?? "sqlite";
            string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            switch (providerType.ToLower())
            {
                case "mysql":
                case "mariadb":
                    builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case "sqlite":
                case "sqlite3":
                    builder.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case "sqlserver":
                case "mssql":
                    builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                default:
                    throw new Exception($"Unknow database provider type: {providerType} - IdentityServer Operational");
            }
        }
    }
}
