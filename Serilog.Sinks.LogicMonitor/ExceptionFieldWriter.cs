using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes exception (just it ToString())
	/// </summary>
	public class ExceptionFieldWriter : FieldWriterBase
	{

		public object? GetValue(LogEvent logEvent)
			=> GetValue(logEvent, null);

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
			=> logEvent.Exception?.ToString();
	}
}