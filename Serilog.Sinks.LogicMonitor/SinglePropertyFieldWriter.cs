using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.IO;
using System.Text;

namespace Serilog.Sinks.LogicMonitor
{
	/// <summary>
	/// Write single event property
	/// </summary>
	public class SinglePropertyFieldWriter(
		string propertyName,
		PropertyWriteMethod writeMethod = PropertyWriteMethod.ToString,
		string? format = null) : FieldWriterBase
	{
		public string Name { get; } = propertyName;

		public PropertyWriteMethod WriteMethod { get; } = writeMethod;

		public string? Format { get; } = format;

		public object? GetValue(LogEvent logEvent)
			=> GetValue(logEvent, null);

		public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		{
			if (!logEvent.Properties.ContainsKey(Name))
			{
				return null;
			}

			switch (WriteMethod)
			{
				case PropertyWriteMethod.Raw:
					return GetPropertyValue(logEvent.Properties[Name]);
				case PropertyWriteMethod.Json:
					var valuesFormatter = new JsonValueFormatter();

					var sb = new StringBuilder();

					using (var writer = new StringWriter(sb))
					{
						valuesFormatter.Format(logEvent.Properties[Name], writer);
					}

					return sb.ToString();

				default:
					return logEvent.Properties[Name].ToString(Format, formatProvider);
			}
		}

		private object? GetPropertyValue(LogEventPropertyValue logEventProperty)
		{
			//TODO: Add support for arrays
			if (logEventProperty is ScalarValue scalarValue)
			{
				return scalarValue.Value;
			}

			return logEventProperty;
		}
	}
}