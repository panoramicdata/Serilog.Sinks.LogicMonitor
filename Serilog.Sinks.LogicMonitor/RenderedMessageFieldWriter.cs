using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes message part
	/// </summary>
	public class RenderedMessageFieldWriter : FieldWriterBase
	{
		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
			=> logEvent.RenderMessage(formatProvider);
	}
}