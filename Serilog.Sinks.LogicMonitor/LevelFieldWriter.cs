using Serilog.Events;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Writes log level
/// </summary>
public class LevelFieldWriter(bool renderAsText) : FieldWriterBase
{
	private readonly bool _renderAsText = renderAsText;


	/// <summary>
	/// Initializes a new instance of the <see cref="LevelFieldWriter"/> class that renders numeric levels.
	/// </summary>
	public LevelFieldWriter() : this(false) { }

	/// <summary>
	/// Gets the level value for the log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>The level name or numeric value depending on configuration.</returns>
	public object? GetValue(LogEvent logEvent) => GetValue(logEvent, null);

	/// <inheritdoc/>
	public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider)
	{
		if (_renderAsText)
		{
			return logEvent.Level.ToString();
		}

		return (int)logEvent.Level;
	}
}