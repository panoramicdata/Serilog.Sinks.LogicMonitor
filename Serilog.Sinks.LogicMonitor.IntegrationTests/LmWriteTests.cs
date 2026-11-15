namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Integration tests for writing log events to LogicMonitor.
/// </summary>
/// <param name="testOutputHelper">The test output helper.</param>
public class LmWriteTests(ITestOutputHelper testOutputHelper) : BaseTest(testOutputHelper)
{
	/// <summary>
	/// Verifies that fifty events can be written with the default fields.
	/// </summary>
	[Fact]
	public void Write50Events_ShouldWrite50EventsToLogicMonitor() => WriteTestEvents(FieldSets.Default(), 50);

	/// <summary>
	/// Verifies that an event containing a null character in JSON can be written.
	/// </summary>
	[Fact]
	public void WriteEventWithZeroCodeCharInJson_ShouldWriteEventToLogicMonitor()
	{
		using var logger = new LoggerConfiguration()
			.WriteTo.LogicMonitor(LogicMonitorClientOptions, ResourceId, FieldSets.WithoutMachineName())
			.CreateLogger();

		logger.Information("Test: {@testObject} testStr: {@testStr:l}", new { IntProp = 42, StringProp = "Test\\u0000" }, "stringValue");
	}

	/// <summary>
	/// Verifies writing with quoted field names.
	/// </summary>
	[Fact]
	public void QuotedFieldNames_ShouldWriteEvents() => WriteTestEvents(FieldSets.WithQuotedNames(), 1);

	/// <summary>
	/// Verifies writing when the single-property writer has no matching property on the event.
	/// </summary>
	[Fact]
	public void MissingSinglePropertyForWriter_ShouldWriteEvents() => WriteTestEvents(FieldSets.WithoutMachineName(), 1);

	/// <summary>
	/// Verifies field names that differ by case.
	/// </summary>
	[Fact]
	public void FieldsWithDifferentCaseNames_ShouldWriteEvents() => WriteTestEvents(FieldSets.WithPascalCaseNames(), 50);
}
