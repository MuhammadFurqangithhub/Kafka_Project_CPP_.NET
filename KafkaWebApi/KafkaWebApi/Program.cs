using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace KafkaWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Ensure librdkafka.dll path is set (if needed)
            Environment.SetEnvironmentVariable(
                "PATH",
                Environment.GetEnvironmentVariable("PATH") + ";" +
                Path.Combine(AppContext.BaseDirectory, "runtimes", "win-x64", "native")
            );

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
