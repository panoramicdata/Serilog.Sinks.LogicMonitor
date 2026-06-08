using Serilog.Events;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes exception (just it ToString())
/// </summary>
public class ExceptionFieldWriter : FieldWriterBase
{

	/// <summary>
	/// Gets the exception text for the log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The exception string, or <see langword="null"/> when no exception exists.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		=> logEvent.Exception?.ToString();
}