using Serilog.Events;
using System;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Writes log level
	/// </summary>
	public partial class LevelFieldWriter : FieldWriterBase
	{
		private readonly bool _renderAsText;

		public LevelFieldWriter(bool renderAsText = false)
		{
			_renderAsText = renderAsText;
		}

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
		{
			if (_renderAsText)
			{
				return logEvent.Level.ToString();
			}

			return (int)logEvent.Level;
		}
	}
}