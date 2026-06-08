using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes all event properties as json
/// </summary>
public class PropertiesFieldWriter : FieldWriterBase
{

	/// <summary>
	/// Gets all event properties as a JSON object string.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>A JSON object string containing all event properties.</returns>
	public object? GetValue(LogEvent logEvent)
		=> GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
		=> PropertiesToJson(logEvent);

	private object PropertiesToJson(LogEvent logEvent)
	{
		if (logEvent.Properties.Count == 0)
		{
			return "{}";
		}

		var valuesFormatter = new JsonValueFormatter();

		var sb = new StringBuilder();

		sb.Append("{");

		using (var writer = new StringWriter(sb))
		{
			foreach (var logEventProperty in logEvent.Properties)
			{
				sb.Append('\"').Append(logEventProperty.Key).Append("\":");

				valuesFormatter.Format(logEventProperty.Value, writer);

				sb.Append(", ");
			}
		}

		sb.Remove(sb.Length - 2, 2);
		sb.Append("}");

		return sb.ToString();
	}
}