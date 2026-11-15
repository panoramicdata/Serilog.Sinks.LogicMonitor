using Microsoft.Extensions.Logging;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

internal sealed class TestOutputLogger(ITestOutputHelper output) : Microsoft.Extensions.Logging.ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		output.WriteLine($"{logLevel}: {formatter(state, exception)}");
		if (exception is not null)
		{
			output.WriteLine(exception.ToString());
		}
	}
}
