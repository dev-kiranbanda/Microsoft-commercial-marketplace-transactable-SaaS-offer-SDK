namespace Microsoft.Marketplace.Saas.Web
{
    using System.Reflection;
    using System.Xml;
    using log4net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using System.IO;
    using System.Linq;
    using MicroKnights.Logging;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
       //.ConfigureLogging(logging =>
       //{
       //    logging.ClearProviders();
       //    logging.AddConsole();
       //})
       .ConfigureWebHostDefaults(webBuilder =>
       {
           webBuilder.UseUrls("https://*:5081", "http://*:5080");
           webBuilder.UseStartup<Startup>();
       });
    }
}
