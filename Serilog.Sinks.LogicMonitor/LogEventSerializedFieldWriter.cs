using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes log event as json
/// </summary>
public class LogEventSerializedFieldWriter : FieldWriterBase
{

	/// <summary>
	/// Gets the full log event as serialized JSON.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The serialized log event payload.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		=> LogEventToJson(logEvent, formatProvider);

	private object LogEventToJson(LogEvent logEvent, IFormatProvider? formatProvider)
	{
		var jsonFormatter = new JsonFormatter(formatProvider: formatProvider);

		var sb = new StringBuilder();
		using (var writer = new StringWriter(sb))
			jsonFormatter.Format(logEvent, writer);
		return sb.ToString();
	}
}