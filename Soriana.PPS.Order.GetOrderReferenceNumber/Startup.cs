using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

using Soriana.PPS.Common.Configuration;
using Soriana.PPS.Common.Extensions;
using Soriana.PPS.DataAccess.Configuration;
using Soriana.PPS.Common.Data;
using Soriana.PPS.DataAccess.Repository;
using Soriana.PPS.Common.Data.Dapper;
using Soriana.PPS.Common.Constants;
using Soriana.PPS.Common.Entities;
using Soriana.PPS.Common.HttpClient;
using Soriana.PPS.DataAccess.PendingPayments;
using Soriana.PPS.Order.GetOrderReferenceNumber.Services;


[assembly: FunctionsStartup(typeof(Soriana.PPS.Order.GetOrderReferenceNumber.Startup))]
namespace Soriana.PPS.Order.GetOrderReferenceNumber
{
    public class Startup : FunctionsStartup
    {
        #region Constructors
        public Startup() { }
        #endregion

        #region Overrides Methods
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Formatter Injection
            builder.Services.AddMvcCore().AddNewtonsoftJson(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

            //Configuration Injection
            IConfiguration configuration = builder.GetContext().Configuration;
            builder.Services.Configure<IConfiguration>(configuration);

            //SeriLog Injection
            builder.Services.AddSeriLogConfiguration(configuration);

            //HttpClient Injection
            builder.Services.AddHttpClient();

            //DataAccess Service Injection -- DataBase
            builder.Services.AddScoped<IDbConnection>(o =>
            {
                PaymentProcessorOptions paymentProcessorOptions = new PaymentProcessorOptions();
                configuration.GetSection(PaymentProcessorOptions.PAYMENT_PROCESSOR_OPTIONS_CONFIGURATION).Bind(paymentProcessorOptions);
                return new SqlConnection(paymentProcessorOptions.ConnectionString);
            });
            //DataAccess Service Injection -- Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(o =>
            {
                return new UnitOfWork(o.GetRequiredService<IDbConnection>());
            });

            //DataAccess Service Injection -- Repositories Operations
            builder.Services.AddScoped<IRepositoryRead<ClientHasToken>, RepositoryRead<ClientHasToken>>();
            builder.Services.AddScoped<IRepositoryCreate<ClientHasToken>, RepositoryWrite<ClientHasToken>>();
            builder.Services.AddSingleton<ICreateTableType, CreateTableTypeBase>();

            //DataAccess Service Injection -- Repositories Operations
            builder.Services.AddScoped<IPaymentStoreRepository, PaymentStoreRepository>();

            //DataAccess Service Injection -- Context
            builder.Services.AddScoped<IPendingPaymentsContext, PendingPaymentsContext>();

            //Business Service Injection -- Service
            builder.Services.AddScoped<IOrderReferenceService, OrderReferenceService>();
        }
        #endregion
    }
}
