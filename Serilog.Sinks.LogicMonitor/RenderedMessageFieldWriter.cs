using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes message part
	/// </summary>
	public class RenderedMessageFieldWriter : FieldWriterBase
	{

		public object? GetValue(LogEvent logEvent)
			=> GetValue(logEvent, null);

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
			=> logEvent.RenderMessage(formatProvider);
	}
}