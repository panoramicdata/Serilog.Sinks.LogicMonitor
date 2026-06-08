namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="LevelFieldWriter"/>.
/// </summary>
public class LevelFieldWriterTest
{
	/// <summary>
	/// Verifies the default numeric level output.
	/// </summary>
	[Fact]
	public void ByDefault_ShouldWriteLevelNo()
	{
		var writer = new LevelFieldWriter();
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().Be(1);
	}

	/// <summary>
	/// Verifies text level output when configured.
	/// </summary>
	[Fact]
	public void WriteAsTextIsTrue_ShouldWriteLevelName()
	{
		var writer = new LevelFieldWriter(true);
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().Be(nameof(LogEventLevel.Debug));
	}
}