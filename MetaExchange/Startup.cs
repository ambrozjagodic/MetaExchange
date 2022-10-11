using MetaExchange.Config;
using MetaExchange.Core;
using MetaExchange.DataSource;
using MetaExchange.Logic;
using MetaExchange.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MetaExchange
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string orderBookFilePath = _configuration.GetValue<string>(SettingsConsts.ORDER_BOOK_FILE_PATH);

            services.AddMvcCore();
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IMetaExchangeLogic, MetaExchangeLogic>();
            services.AddSingleton<ISequenceFinder, SequenceFinder>();
            services.AddSingleton<ISequenceFinderHelper, SequenceFinderHelper>();
            services.AddSingleton<IOutputWriter, ConsoleWriter>();
            services.AddSingleton<IWebAPIRequestValidation, WebAPIRequestValidation>();
            services.AddSingleton<IMetaExchangeDataSource>(new MetaExchangeDataSource(new FileOrderBookReader(), new OrderBookDataFactory(), new ConsoleWriter(), orderBookFilePath));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(context =>
            {
                return context.Response.WriteAsync("Up and running.");
            });
        }
    }
}