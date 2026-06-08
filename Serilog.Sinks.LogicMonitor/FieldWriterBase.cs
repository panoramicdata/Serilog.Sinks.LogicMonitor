using Serilog.Events;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Base type for extracting values from a <see cref="LogEvent"/>.
/// </summary>
public abstract class FieldWriterBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="FieldWriterBase"/> class.
	/// </summary>
	protected FieldWriterBase()
	{
	}

	/// <summary>
	/// Gets part of log event to write to the column
	/// </summary>
	/// <param name="logEvent">The log event to extract the value from.</param>
	/// <param name="formatProvider">The optional format provider used for rendering.</param>
	/// <returns>The extracted value to write, or <see langword="null"/>.</returns>
	public abstract object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null);
}