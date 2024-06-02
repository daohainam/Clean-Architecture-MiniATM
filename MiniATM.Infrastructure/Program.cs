
using AutoMapper;
using Microsoft.Extensions.Configuration;
using MiniATM.Infrastructure.Models;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.DataContext;
using MiniATM.Infrastructure.SqlServer.Repositories.SqlServer.MapperProfile;
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
        var connectionString = configuration.GetConnectionString("MiniATMDatabase");
        var repositoryOptions = configuration.GetSection("RepositoryOptions").Get<RepositoryOptions>() ?? throw new Exception("No RepositoryOptions found");

        if (repositoryOptions.RepositoryType == RepositoryTypes.SqlServer)
        {
            services.AddAutoMapper(typeof(SqlServer2EntityProfile));

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
            // we need to implement remote repositories and register here
        }
    }
}
