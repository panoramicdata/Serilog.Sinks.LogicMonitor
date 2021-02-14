using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes timestamp part
	/// </summary>
	public class TimestampFieldWriter : FieldWriterBase
	{
		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
			=> logEvent.Timestamp;
	}
}