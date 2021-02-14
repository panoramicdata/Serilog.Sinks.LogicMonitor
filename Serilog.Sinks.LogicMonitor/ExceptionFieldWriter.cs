using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes exception (just it ToString())
	/// </summary>
	public class ExceptionFieldWriter : FieldWriterBase
	{
		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
			=> logEvent.Exception?.ToString();
	}
}