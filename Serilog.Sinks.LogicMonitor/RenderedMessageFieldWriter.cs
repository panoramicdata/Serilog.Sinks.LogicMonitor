using Serilog.Events;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes message part
/// </summary>
public class RenderedMessageFieldWriter : FieldWriterBase
{

	/// <summary>
	/// Gets the rendered message for the log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The rendered message text.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		=> logEvent.RenderMessage(formatProvider);
}