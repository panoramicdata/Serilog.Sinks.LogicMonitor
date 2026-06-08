namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="TimestampFieldWriter"/>.
/// </summary>
public class TimestampColumnWriterTest
{
	/// <summary>
	/// Verifies timestamp values are returned unchanged by default.
	/// </summary>
	[Fact]
	public void ByDefault_ShouldReturnTimestampValueWithoutTimezone()
	{
		var writer = new TimestampFieldWriter();
		var timeStamp = new DateTimeOffset(2017, 8, 13, 11, 11, 11, new TimeSpan());
		var testEvent = new LogEvent(timeStamp, LogEventLevel.Debug, null, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().Be(timeStamp);
	}

	/// <summary>
	/// Verifies timestamp values are returned for timezone-aware scenarios.
	/// </summary>
	[Fact]
	public void DbTypeWithTimezoneSelected_ShouldReturnTimestampValue()
	{
		var writer = new TimestampFieldWriter();
		var timeStamp = new DateTimeOffset(2017, 8, 13, 11, 11, 11, new TimeSpan());
		var testEvent = new LogEvent(timeStamp, LogEventLevel.Debug, null, new MessageTemplate([]), []);

		var result = writer.GetValue(testEvent);

		result.Should().Be(timeStamp);
	}
}
