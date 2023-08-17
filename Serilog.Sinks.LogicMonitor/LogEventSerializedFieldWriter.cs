using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.IO;
using System.Text;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes log event as json
	/// </summary>
	public class LogEventSerializedFieldWriter : FieldWriterBase
	{

		public object? GetValue(LogEvent logEvent)
			=> GetValue(logEvent, null);

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
}