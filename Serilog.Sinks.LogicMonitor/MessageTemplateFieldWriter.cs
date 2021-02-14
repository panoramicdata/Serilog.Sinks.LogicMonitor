using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes non rendered message
	/// </summary>
	public class MessageTemplateFieldWriter : FieldWriterBase
	{
		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
			=> logEvent.MessageTemplate.Text;
	}
}