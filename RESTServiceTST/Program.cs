using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RESTServiceTST
{
	public class Program
	{
		private const string ConsoleParam = "--console";
		private const string AppSettings = "appsettings.json";
		private static readonly string EnvAppSettings = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";

		private static IConfigurationBuilder AppConfigurationBuilder =>
			new ConfigurationBuilder()
				.AddJsonFile(AppSettings, false, true)
				.AddJsonFile(EnvAppSettings, true, true)
				.AddEnvironmentVariables();

		public static void Main(string[] args)
		{
			var configuration = AppConfigurationBuilder.Build();
			Log.Logger = new LoggerConfiguration().CreateLogger();

			try
			{
				Log.Information($"Env: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

				var isService = !(Debugger.IsAttached || ((IList)args).Contains(ConsoleParam));

				if (isService)
				{
					var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
					var pathToContentRoot = Path.GetDirectoryName(pathToExe);
					Directory.SetCurrentDirectory(pathToContentRoot);
					Log.Information($"Content path: {pathToContentRoot}");
				}

				var builder = CreateWebHostBuilder(configuration, args.Where(arg => arg != ConsoleParam).ToArray());

				var host = builder.Build();

				if (isService)
				{
					host.Run();// .RunAsService();
				}
				else
				{
					host.Run();
				}
			}
			catch (Exception e)
			{
				Log.Error($"Error occurred while initializing {Assembly.GetExecutingAssembly().FullName} - {e.Message}", e);

				throw;
			}
		}

		private static IWebHostBuilder CreateWebHostBuilder(IConfiguration configuration, string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(configuration)
				.UseStartup<Startup>()
				.CaptureStartupErrors(true);
	}
}
