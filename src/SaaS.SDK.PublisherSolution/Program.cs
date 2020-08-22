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
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            ILog _databaseLogger = log4net.LogManager.GetLogger(typeof(Program));
            var repository = _databaseLogger?.Logger.Repository;

            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
            var logConnectionString = config.GetSection("SaasApiConfiguration:ApplicationLogConnection")?.Value;

            if (repository != null)
            {
                var _adoAppender = repository.GetAppenders()
                     .FirstOrDefault(a => a is AdoNetAppender) as AdoNetAppender;

                if (_adoAppender != null && string.IsNullOrEmpty(_adoAppender.ConnectionStringName))
                {
                    _adoAppender.ConnectionString = logConnectionString;
                    _adoAppender.ActivateOptions();
                    _databaseLogger.Info("Logger Activated");
                }
                CreateHostBuilder(args).Build().Run();
            }
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
