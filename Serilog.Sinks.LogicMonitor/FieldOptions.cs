namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Provides reusable field writer configurations.
/// </summary>
public static class FieldOptions
{
	/// <summary>
	/// Gets the default field writer map used by the sink.
	/// </summary>
	public static IDictionary<string, FieldWriterBase> Default => new Dictionary<string, FieldWriterBase>
	  {
			{DefaultFieldNames.RenderedMesssage,new RenderedMessageFieldWriter()},
			{DefaultFieldNames.MessageTemplate, new MessageTemplateFieldWriter()},
			{DefaultFieldNames.Level, new LevelFieldWriter()},
			{DefaultFieldNames.Timestamp, new TimestampFieldWriter()},
			{DefaultFieldNames.Exception, new ExceptionFieldWriter()},
			{DefaultFieldNames.LogEventSerialized, new LogEventSerializedFieldWriter()}
	  };
}