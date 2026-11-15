namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="TimestampFieldWriter"/>.
/// </summary>
public class TimestampColumnWriterTest
{
	/// <summary>
	/// Verifies the event timestamp is returned unchanged, whatever its offset.
	/// </summary>
	[Theory]
	[InlineData(0)]
	[InlineData(5)]
	[InlineData(-8)]
	public void GetValue_ReturnsTheEventTimestampUnchanged(int offsetHours)
	{
		var timeStamp = new DateTimeOffset(2017, 8, 13, 11, 11, 11, TimeSpan.FromHours(offsetHours));
		var testEvent = new LogEvent(timeStamp, LogEventLevel.Debug, null, new MessageTemplate([]), []);

		new TimestampFieldWriter().GetValue(testEvent).Should().Be(timeStamp);
	}
}
