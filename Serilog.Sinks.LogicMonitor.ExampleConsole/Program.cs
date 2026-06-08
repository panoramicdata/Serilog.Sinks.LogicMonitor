using Microsoft.Extensions.Configuration;

namespace Serilog.Sinks.LogicMonitor.ExampleConsole;

/// <summary>
/// Example console entry point for exercising the LogicMonitor sink.
/// </summary>
public class Program
{
	/// <summary>
	/// Application entry point.
	/// </summary>
	public static void Main()
	{
		var basePath = Directory.GetCurrentDirectory();
		var fileInfo = new FileInfo(Path.Combine(basePath, "../../../appsettings.json"));

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
