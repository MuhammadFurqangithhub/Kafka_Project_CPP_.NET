using KafkaWebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace KafkaWebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers to the container
            services.AddControllers();

            // Register Kafka services (Producer and Consumer)
            services.AddSingleton<KafkaProducer>(); // This is the service for producing messages to Kafka
            services.AddSingleton<MessageStore>(); // ✅
            services.AddSingleton<KafkaConsumer>(); // Register KafkaConsumer as singleton
            services.AddHostedService(provider => provider.GetService<KafkaConsumer>()); // Run as background service


            // Register Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KafkaWebApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable Swagger and the Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KafkaWebApi v1"));
            }

            // Enable HTTPS redirection
            app.UseHttpsRedirection();

            // Use routing for API requests
            app.UseRouting();

            // Use Authorization if required
            app.UseAuthorization();

            // Map the controllers for the API
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
