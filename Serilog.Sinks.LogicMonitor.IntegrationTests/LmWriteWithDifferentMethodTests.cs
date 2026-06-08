namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Tests for writing events to LogicMonitor using different methods of specifying the target resource, including by resource ID, resource display name, custom property, and a dictionary of properties. These tests ensure that the sink can correctly identify the target resource using various approaches and that events are written successfully. Note that these tests currently do not verify that the events were actually received in LogicMonitor, so they should be used in conjunction with manual verification or additional tests that query LogicMonitor for the events after writing them.
/// </summary>
/// <param name="testOutputHelper"></param>
public class LmWriteWithDifferentMethodTests(ITestOutputHelper testOutputHelper) : BaseTest(testOutputHelper)
{
	/// <summary>
	/// Tests writing an event to LogicMonitor by specifying the target resource using its resource ID. This test verifies that the sink can successfully write an event when the resource ID is provided, but does not currently verify that the event was received in LogicMonitor. To fully validate this functionality, additional steps would be needed to query LogicMonitor for the event after it is written and confirm that it was associated with the correct resource.
	/// </summary>
	[Fact]
	public void WriteEvent_UsingResourceId_ShouldSucceed()
	{
		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			ResourceId,
			FieldProperties
		)
			.Enrich
			.WithMachineName()
			.CreateLogger();

		logger.Information($"Serilog.Sinks.LogicMonitor: {nameof(WriteEvent_UsingResourceId_ShouldSucceed)}");

		// TODO - Test it worked
	}

	/// <summary>
	/// Tests writing an event to LogicMonitor by specifying the target resource using its display name. This test verifies that the sink can successfully write an event when the resource display name is provided, but does not currently verify that the event was received in LogicMonitor. To fully validate this functionality, additional steps would be needed to query LogicMonitor for the event after it is written and confirm that it was associated with the correct resource. Additionally, this test assumes that resource display names are unique within the LogicMonitor account, which may not always be the case, so it may be necessary to ensure that the display name used in the test is unique or to enhance the sink to handle non-unique display names appropriately.
	/// </summary>
	[Fact]
	public async Task WriteEvent_UsingResourceDisplayName_ShouldSucceed()
	{
		var logicMonitorClient = new LogicMonitorClient(LogicMonitorClientOptions);
		var resource = await logicMonitorClient.GetAsync<Resource>(ResourceId, default);

		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			resource.DisplayName,
			FieldProperties
		)
			.Enrich
			.WithMachineName()
			.CreateLogger();

		logger.Information($"Serilog.Sinks.LogicMonitor: {nameof(WriteEvent_UsingResourceDisplayName_ShouldSucceed)}");

		// TODO - Test it worked
	}

	/// <summary>
	/// Tests writing an event to LogicMonitor by specifying the target resource using a custom property value. This test retrieves a custom property (in this case, "cmdb.id") from the target resource and uses its name and value to identify the resource when writing the event. This approach allows for more flexibility in identifying resources, especially in cases where resource IDs or display names may not be unique or easily accessible. However, it also requires that the target resource has the specified custom property configured, so the test includes a check to ensure that the custom property exists before attempting to write the event. As with the other tests, this test does not currently verify that the event was received in LogicMonitor, so additional steps would be needed to confirm that the event was associated with the correct resource after it is written.
	/// </summary>
	/// <exception cref="FormatException"></exception>
	[Fact]
	public async Task WriteEvent_UsingResourceCustomProperty_ShouldSucceed()
	{
		var logicMonitorClient = new LogicMonitorClient(LogicMonitorClientOptions);
		var resource = await logicMonitorClient.GetAsync<Resource>(ResourceId, default);

		// Get the "cmdb.id" custom property from the device
		var customProperty = resource
			.CustomProperties
			.FirstOrDefault(cp => cp.Name == "cmdb.id")
			?? throw new FormatException($"Resource {resource.DisplayName} is missing the cmdb.id custom property needed for this unit test.");

		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			customProperty.Name,
			customProperty.Value,
			FieldProperties
		)
			.Enrich
			.WithMachineName()
			.CreateLogger();

		logger.Information($"Serilog.Sinks.LogicMonitor: {nameof(WriteEvent_UsingResourceCustomProperty_ShouldSucceed)}");

		// TODO - Test it worked
	}

	/// <summary>
	/// Tests writing an event to LogicMonitor by specifying the target resource using a dictionary of properties. This test constructs a dictionary that includes the resource ID, display name, and a custom property value, and uses this dictionary to identify the target resource when writing the event. This method provides the most flexibility in identifying resources, as it allows for multiple properties to be used in combination to ensure that the correct resource is targeted. However, it also requires that the sink be able to handle the logic of matching the provided properties to the correct resource in LogicMonitor, which may involve additional complexity compared to using a single identifier like resource ID or display name. As with the other tests, this test does not currently verify that the event was received in LogicMonitor, so additional steps would be needed to confirm that the event was associated with the correct resource after it is written.
	/// </summary>
	/// <exception cref="FormatException"></exception>
	[Fact]
	public async Task WriteEvent_UsingPropertyDictionary_ShouldSucceed()
	{
		var logicMonitorClient = new LogicMonitorClient(LogicMonitorClientOptions);
		var resource = await logicMonitorClient.GetAsync<Resource>(ResourceId, default);

		// Get the "cmdb.id" custom property from the device
		var customProperty = resource
			.CustomProperties
			.FirstOrDefault(cp => cp.Name == "cmdb.id")
			?? throw new FormatException($"Resource {resource.DisplayName} is missing the cmdb.id custom property needed for this unit test.");


		var propertyDictionary = new Dictionary<string, string>
		{
			{ "system.deviceId", resource.Id.ToString(CultureInfo.InvariantCulture) },
			{ "system.displayname", resource.DisplayName },
			{ "cmdb.id", customProperty.Value }
		};

		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			propertyDictionary,
			FieldProperties
		)
			.Enrich
			.WithMachineName()
			.CreateLogger();

		logger.Information($"Serilog.Sinks.LogicMonitor: {nameof(WriteEvent_UsingPropertyDictionary_ShouldSucceed)}");

		// TODO - Test it worked
	}
}