namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Provides the default output field names used by the sink.
/// </summary>
public static class DefaultFieldNames
{
	/// <summary>
	/// Field name for the rendered message text.
	/// </summary>
	public static readonly string RenderedMesssage = "message";

	/// <summary>
	/// Field name for the unrendered message template.
	/// </summary>
	public static readonly string MessageTemplate = "message_template";

	/// <summary>
	/// Field name for the log level.
	/// </summary>
	public static readonly string Level = "level";

	/// <summary>
	/// Field name for the event timestamp.
	/// </summary>
	public static readonly string Timestamp = "timestamp";

	/// <summary>
	/// Field name for exception details.
	/// </summary>
	public static readonly string Exception = "exception";

	/// <summary>
	/// Field name for the serialized log event JSON payload.
	/// </summary>
	public static readonly string LogEventSerialized = "log_event";
}