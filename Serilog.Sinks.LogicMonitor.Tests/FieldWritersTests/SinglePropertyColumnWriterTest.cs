namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests;

/// <summary>
/// Tests for <see cref="SinglePropertyFieldWriter"/>.
/// </summary>
public class SinglePropertyColumnWriterTest
{
	private const string PropertyName = "TestProperty";

	private static object? GetValue(SinglePropertyFieldWriter writer, params LogEventProperty[] properties)
		=> writer.GetValue(new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate([]), properties));

	private static LogEventProperty Scalar(object value) => new(PropertyName, new ScalarValue(value));

	/// <summary>
	/// Verifies the format is respected when writing with ToString.
	/// </summary>
	[Fact]
	public void WithToStringSelected_ShouldRespectFormatPassed()
		=> GetValue(new SinglePropertyFieldWriter(PropertyName, PropertyWriteMethod.ToString, format: "l"), Scalar("TestValue"))
			.Should().Be("TestValue");

	/// <summary>
	/// Verifies missing properties return null.
	/// </summary>
	[Fact]
	public void PropertyIsNotPresent_ShouldReturnNullValue()
		=> GetValue(new SinglePropertyFieldWriter(PropertyName)).Should().BeNull();

	/// <summary>
	/// Verifies scalar values are returned directly in raw mode.
	/// </summary>
	[Fact]
	public void RawSelectedForScalarProperty_ShouldReturnPropertyValue()
		=> GetValue(new SinglePropertyFieldWriter(PropertyName, PropertyWriteMethod.Raw), Scalar(42)).Should().Be(42);

	/// <summary>
	/// Verifies non-scalar values are returned as the property value itself in raw mode.
	/// </summary>
	[Fact]
	public void RawSelectedForStructureProperty_ShouldReturnTheLogEventValue()
	{
		var structure = new StructureValue([new LogEventProperty("Inner", new ScalarValue(1))]);

		GetValue(new SinglePropertyFieldWriter(PropertyName, PropertyWriteMethod.Raw), new LogEventProperty(PropertyName, structure))
			.Should().BeSameAs(structure);
	}

	/// <summary>
	/// Verifies values are serialised as JSON in JSON mode.
	/// </summary>
	[Fact]
	public void JsonSelected_ShouldReturnTheValueAsJson()
		=> GetValue(new SinglePropertyFieldWriter(PropertyName, PropertyWriteMethod.Json), Scalar("TestValue"))
			.Should().Be("\"TestValue\"");

	/// <summary>
	/// Verifies the writer exposes how it was configured.
	/// </summary>
	[Fact]
	public void Constructor_ExposesTheConfiguration()
	{
		var writer = new SinglePropertyFieldWriter(PropertyName, PropertyWriteMethod.Json, "x");

		(writer.Name, writer.WriteMethod, writer.Format).Should().Be((PropertyName, PropertyWriteMethod.Json, "x"));
	}
}
