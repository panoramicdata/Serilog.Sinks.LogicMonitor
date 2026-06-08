using Serilog.Events;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes timestamp part
/// </summary>
public class TimestampFieldWriter : FieldWriterBase
{

	/// <summary>
	/// Gets the timestamp value for the log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The event timestamp.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		=> logEvent.Timestamp;
}