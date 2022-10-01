using MetaExchange.DataSource;
using MetaExchange.Logic;
using MetaExchange.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MetaExchange
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string path = @"D:\Work\Private\MetaExchangeTask\metaExchangeTask_backend_slo\order_books_data";

            services.AddMvcCore();
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IMetaExchangeLogic, MetaExchangeLogic>();
            services.AddSingleton<ISequenceFinder, SequenceFinder>();
            services.AddSingleton<IWebAPIRequestValidation, WebAPIRequestValidation>();
            services.AddSingleton<IMetaExchangeDataSource>(new MetaExchangeDataSource(new FileOrderBookReader(), new ConsoleWriter(), path));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors();

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