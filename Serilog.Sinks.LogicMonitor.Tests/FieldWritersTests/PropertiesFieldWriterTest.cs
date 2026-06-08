namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="PropertiesFieldWriter"/>.
/// </summary>
public class PropertiesFieldWriterTest
{
	/// <summary>
	/// Verifies that an event with no properties serializes to an empty JSON object.
	/// </summary>
	[Fact]
	public void NoProperties_ShouldReturnEmptyJsonObject()
	{
		var writer = new PropertiesFieldWriter();
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().Be("{}");
	}
}