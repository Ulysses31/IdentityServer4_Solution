using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace IdentityServerClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
		 .MinimumLevel.Debug()
		 .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
		 .MinimumLevel.Override("System", LogEventLevel.Debug)
		 .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
		 .Enrich.FromLogContext()
		 // uncomment to write to Azure diagnostics stream
		 //.WriteTo.File(
		 //    @"D:\home\LogFiles\Application\identityserver.txt",
		 //    fileSizeLimitBytes: 1_000_000,
		 //    rollOnFileSizeLimit: true,
		 //    shared: true,
		 //    flushToDiskInterval: TimeSpan.FromSeconds(1))
		 .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
		 .CreateLogger();

			Log.Information("Starting API...");

			var host = CreateHostBuilder(args);

			try
			{
				// run the web app
				host.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "API terminated unexpectedly.");
				throw;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHost CreateHostBuilder(string[] args) =>
				WebHost.CreateDefaultBuilder(args)
				.UseSerilog()
				.UseStartup<Startup>()
				.Build();
	}
}