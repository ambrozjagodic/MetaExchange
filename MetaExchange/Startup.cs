using MetaExchange.DataSource;
using MetaExchange.Logic;
using MetaExchange.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;

namespace MetaExchange
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string path = @"/Users/ambroz/Documents/Projects/MetaExchange/Data/Dataset/order_books_data";

            string runEnvironment = Environment.GetEnvironmentVariable("run-env");
            if (runEnvironment == "e2e-tests")
            {
                path = @"/Users/ambroz/Documents/Projects/MetaExchange/Data/Dataset/order_books_data_test";
            }

            services.AddMvcCore();
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IMetaExchangeLogic, MetaExchangeLogic>();
            services.AddSingleton<ISequenceFinder, SequenceFinder>();
            services.AddSingleton<IOutputWriter, ConsoleWriter>();
            services.AddSingleton<IWebAPIRequestValidation, WebAPIRequestValidation>();
            services.AddSingleton<IMetaExchangeDataSource>(new MetaExchangeDataSource(new FileOrderBookReader(), new ConsoleWriter(), path));
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