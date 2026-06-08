namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Defines how a selected event property should be written.
/// </summary>
public enum PropertyWriteMethod
{
	/// <summary>
	/// Writes the underlying scalar value when possible.
	/// </summary>
	Raw = 0,
	/// <summary>
	/// Writes the value using <c>ToString()</c> formatting.
	/// </summary>
	ToString = 1,
	/// <summary>
	/// Writes the value as JSON.
	/// </summary>
	Json = 2
}