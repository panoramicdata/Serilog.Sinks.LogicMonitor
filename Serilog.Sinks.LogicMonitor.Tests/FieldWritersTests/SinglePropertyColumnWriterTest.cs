namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="SinglePropertyFieldWriter"/>.
/// </summary>
public class SinglePropertyColumnWriterTest
{
	/// <summary>
	/// Verifies <see cref="PropertyWriteMethod.ToString"/> output with formatting.
	/// </summary>
	[Fact]
	public void WithToStringSeleted_ShouldRespectFormatPassed()
	{
		const string propertyName = "TestProperty";
		const string propertyValue = "TestValue";

		var property = new LogEventProperty(propertyName, new ScalarValue(propertyValue));
		var writer = new SinglePropertyFieldWriter(propertyName, PropertyWriteMethod.ToString, format: "l");
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), [property]);
		var result = writer.GetValue(testEvent);

		result.Should().Be(propertyValue);
	}

	/// <summary>
	/// Verifies missing properties return null.
	/// </summary>
	[Fact]
	public void PropertyIsNotPeresent_ShouldReturnNullValue()
	{
		const string propertyName = "TestProperty";
		const string propertyValue = "TestValue";

		_ = new LogEventProperty(propertyName, new ScalarValue(propertyValue));
		var writer = new SinglePropertyFieldWriter(propertyName, PropertyWriteMethod.ToString, format: "l");
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), []);
		var result = writer.GetValue(testEvent);

		result.Should().BeNull();
	}

	/// <summary>
	/// Verifies scalar values are returned directly in raw mode.
	/// </summary>
	[Fact]
	public void RawSelectedForScalarProperty_ShouldReturnPropertyValue()
	{
		const string propertyName = "TestProperty";
		const int propertyValue = 42;

		var property = new LogEventProperty(propertyName, new ScalarValue(propertyValue));
		var writer = new SinglePropertyFieldWriter(propertyName, PropertyWriteMethod.Raw);
		var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), [property]);
		var result = writer.GetValue(testEvent);

		result.Should().Be(propertyValue);
	}
}