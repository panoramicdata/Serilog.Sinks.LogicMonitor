using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Write single event property
/// </summary>
public class SinglePropertyFieldWriter(
	string propertyName,
	PropertyWriteMethod writeMethod = PropertyWriteMethod.ToString,
	string? format = null) : FieldWriterBase
{
	/// <summary>
	/// Gets the property name to extract.
	/// </summary>
	public string Name { get; } = propertyName;

	/// <summary>
	/// Gets the write strategy for the property.
	/// </summary>
	public PropertyWriteMethod WriteMethod { get; } = writeMethod;

	/// <summary>
	/// Gets the optional format string used when writing with <see cref="PropertyWriteMethod.ToString"/>.
	/// </summary>
	public string? Format { get; } = format;

	/// <summary>
	/// Gets the configured property value from a log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The resolved property value, or <see langword="null"/> if not found.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
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