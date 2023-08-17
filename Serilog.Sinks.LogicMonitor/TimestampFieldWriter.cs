using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes timestamp part
	/// </summary>
	public class TimestampFieldWriter : FieldWriterBase
	{

		public object? GetValue(LogEvent logEvent)
			=> GetValue(logEvent, null);

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
			=> logEvent.Timestamp;
	}
}