namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="ExceptionFieldWriter"/>.
/// </summary>
public class ExceptionColumnWriterTest
{
	/// <summary>
	/// Verifies that no exception returns a null value.
	/// </summary>
	[Fact]
	public void ExceptionIsNull_ShouldReturnNullValue()
	{
		var writer = new ExceptionFieldWriter();
		var testEvent = new LogEvent(
			DateTime.Now,
			LogEventLevel.Debug,
			null, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().BeNull();
	}

	/// <summary>
	/// Verifies that an exception is returned as a string.
	/// </summary>
	[Fact]
	public void ExceptionIsPresent_ShouldReturnStringrepresentation()
	{
		var writer = new ExceptionFieldWriter();
		var exception = new Exception("Test exception");
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, exception, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().Be(exception.ToString());
	}
}