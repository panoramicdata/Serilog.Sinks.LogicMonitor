using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	public abstract class FieldWriterBase
	{
		protected FieldWriterBase()
		{
		}

		/// <summary>
		/// Gets part of log event to write to the column
		/// </summary>
		/// <param name="logEvent"></param>
		/// <param name="formatProvider"></param>
		public abstract object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null);
	}
}