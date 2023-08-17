using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes non rendered message
	/// </summary>
	public class MessageTemplateFieldWriter : FieldWriterBase
	{

		public object? GetValue(LogEvent logEvent)
			=> GetValue(logEvent, null);

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
			=> logEvent.MessageTemplate.Text;
	}
}