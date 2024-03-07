using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes log level
	/// </summary>
	public class LevelFieldWriter(bool renderAsText) : FieldWriterBase
	{
		private readonly bool _renderAsText = renderAsText;


		public LevelFieldWriter() : this(false) { }

		public object? GetValue(LogEvent logEvent) => GetValue(logEvent, null);

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		{
			if (_renderAsText)
			{
				return logEvent.Level.ToString();
			}

			return (int)logEvent.Level;
		}
	}
}