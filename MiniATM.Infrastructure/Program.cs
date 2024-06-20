
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniATM.Infrastructure.CashStorage;
using MiniATM.Infrastructure.InMemory;
using MiniATM.Infrastructure.Models;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.DataContext;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.MapperProfile;
using MiniATM.UseCase;
using MiniATM.UseCase.Caching;
using MiniATM.UseCase.Repositories;
using MiniATM.UseCase.UnitOfWork;

namespace MiniATM.Infrastructure;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        RegisterInfrastructureServices(builder.Configuration, builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    private static void RegisterInfrastructureServices(ConfigurationManager configuration, IServiceCollection services)
    {
        var repositoryOptions = configuration.GetSection("Repository").Get<RepositoryOptions>() ?? throw new Exception("No RepositoryOptions found");

        if (repositoryOptions.RepositoryType == RepositoryTypes.SqlServer)
        {
            services.AddAutoMapper(typeof(SqlServer2EntityProfile));

            services.AddDbContext<MiniATMContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("MiniATMDatabase")));

            services.AddTransient<IBankAccountRepository>(services => new SqlServerBankAccountRepository(
                services.GetRequiredService<MiniATMContext>(), services.GetRequiredService<IMapper>()
                ));
            services.AddTransient<ICustomerRepository>(services => new SqlServerCustomerRepository(
                services.GetRequiredService<MiniATMContext>(), services.GetRequiredService<IMapper>()
                ));
            services.AddTransient<ITransactionRepository>(services => new SqlServerTransactionRepository(
                services.GetRequiredService<MiniATMContext>(), services.GetRequiredService<IMapper>()
                ));
            services.AddTransient<ITransactionUnitOfWork>(services => new SqlServerTransactionUnitOfWork(
                services.GetRequiredService<MiniATMContext>(), services.GetRequiredService<IMapper>()
                ));
        }
        else
        {
            services.AddTransient<IBankAccountRepository>(services => new InMemoryBankAccountRepository());
            services.AddTransient<ICustomerRepository>(services => new InMemoryCustomerRepository());
            services.AddTransient<ITransactionRepository>(services => new InMemoryTransactionRepository());
            services.AddTransient<ITransactionUnitOfWork>(services => new InMemoryTransactionUnitOfWork());
        }

        var cacheOptions = configuration.GetSection("Cache").Get<CacheOptions>() ?? new CacheOptions();
        InitializeCache(services, configuration, cacheOptions);

        services.AddSingleton<ICashStorage>(services => new InMemoryCashStorage(
            services.GetRequiredService<ILogger<InMemoryCashStorage>>(),
            5000
            )); // must be singleton

        services.AddTransient<IBankAccountFinder>(services => new CachableBankAccountFinder(new RepositoryBankAccountFinder(
            services.GetRequiredService<IBankAccountRepository>()
            ),
            services.GetRequiredService<IDistributedCache>(),
            configuration.GetSection("CachableBankAccountFinderOptions").Get<CachableBankAccountFinderOptions>() ?? new(),
            services.GetRequiredService<ILogger<CachableBankAccountFinder>>()
            ));

        services.AddTransient<ICashWithdrawalManager>(services => new CashWithdrawalManager(
            services.GetRequiredService<ITransactionUnitOfWork>(),
            services.GetRequiredService<ICashStorage>(),
            true
            ));

        services.AddTransient<ITransferManager>(services => new TransferManager(
            services.GetRequiredService<ITransactionUnitOfWork>()
            ));
    }

    private static void InitializeCache(IServiceCollection services, ConfigurationManager configuration, CacheOptions cacheOptions)
    {
        switch (cacheOptions.Type) { 
            case CacheTypes.Memory:
                services.AddDistributedMemoryCache();
                break;
            case CacheTypes.SqlServer:
                /*
                 * Run this SQL to create cache table: dotnet sql-cache create <connection string> dbo <cache table>
                 */

                if (cacheOptions.SqlServerOptions == null)
                {
                    throw new Exception("Missing option: CachingOptions:SqlServer");
                }
                services.AddDistributedSqlServerCache(options => {
                    options.ConnectionString = configuration.GetConnectionString(cacheOptions.SqlServerOptions.ConnectionStringName);
                    options.TableName = cacheOptions.SqlServerOptions.TableName;
                    options.SchemaName = cacheOptions.SqlServerOptions.SchemaName;
                });
                break;
            case CacheTypes.Redis:
                if (cacheOptions.RedisOptions == null)
                {
                    throw new Exception("Missing option: CachingOptions:Redis");
                }
                services.AddStackExchangeRedisCache(options => {
                    options.Configuration = configuration.GetConnectionString(cacheOptions.RedisOptions.ConnectionStringName);
                });
                break;
            default: 
                throw new Exception("Unknown cache type");
        }
    }
}
