// Use IHostBuilder to configure Serilog and LogicMonitorSink

using Microsoft.Extensions.Configuration;
using Serilog;

public class Program
{
	public static void Main(string[] args)
	{
		var basePath = Directory.GetCurrentDirectory();
		var fileInfo = new FileInfo(Path.Combine(basePath, "../../../appsettings.json"));
		var fileExists = fileInfo.Exists;
		var configuration = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile(fileInfo.FullName)
			.AddEnvironmentVariables()
			.Build();

		var logger = new LoggerConfiguration()
			.ReadFrom
			.Configuration(configuration)
			.CreateLogger();

		logger.Information("Hello, Serilog.Sinks.LogicMonitor.ExampleConsole");
	}
}
