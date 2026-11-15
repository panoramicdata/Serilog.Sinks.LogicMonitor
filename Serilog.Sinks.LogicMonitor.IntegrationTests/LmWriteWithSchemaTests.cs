namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Integration tests for writing log events with the default field set.
/// </summary>
/// <param name="testOutputHelper">The test output helper.</param>
public class LmWriteWithSchemaTests(ITestOutputHelper testOutputHelper) : BaseTest(testOutputHelper)
{
	/// <summary>
	/// Verifies that writing an error event succeeds.
	/// </summary>
	[Fact]
	public void WriteAnErrorEvent_ShouldSucceed()
	{
		using var logger = new LoggerConfiguration()
			.WriteTo.LogicMonitor(LogicMonitorClientOptions, ResourceId, FieldProperties)
			.Enrich.WithMachineName()
			.CreateLogger();

		logger.Error("Error Writing Test: {@testObject}", new { IntProp = 42, StringProp = "Test" });
	}

	/// <summary>
	/// Verifies that fifty events are written successfully.
	/// </summary>
	[Fact]
	public void Write50Events_ShouldSucceed() => WriteTestEvents(FieldProperties, 50);
}
