using Serilog.Events;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes non rendered message
/// </summary>
public class MessageTemplateFieldWriter : FieldWriterBase
{

	/// <summary>
	/// Gets the unrendered message template text for the log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The message template text.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		=> logEvent.MessageTemplate.Text;
}