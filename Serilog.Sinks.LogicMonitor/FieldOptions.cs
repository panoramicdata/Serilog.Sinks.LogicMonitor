using System.Collections.Generic;

namespace Serilog.Sinks.LogicMonitor
{
	public static class FieldOptions
	{
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
}