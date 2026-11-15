namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Integration tests for identifying the target resource by id, display name, custom property and property dictionary.
/// </summary>
/// <param name="testOutputHelper">The test output helper.</param>
public class LmWriteWithDifferentMethodTests(ITestOutputHelper testOutputHelper) : BaseTest(testOutputHelper)
{
	/// <summary>
	/// Writes an event identifying the resource by id.
	/// </summary>
	[Fact]
	public void WriteEvent_UsingResourceId_ShouldSucceed()
		=> WriteOneEvent(sink => sink.LogicMonitor(LogicMonitorClientOptions, ResourceId, FieldProperties), nameof(WriteEvent_UsingResourceId_ShouldSucceed));

	/// <summary>
	/// Writes an event identifying the resource by display name.
	/// </summary>
	[Fact]
	public async Task WriteEvent_UsingResourceDisplayName_ShouldSucceed()
	{
		var resource = await GetResourceAsync();

		WriteOneEvent(sink => sink.LogicMonitor(LogicMonitorClientOptions, resource.DisplayName, FieldProperties), nameof(WriteEvent_UsingResourceDisplayName_ShouldSucceed));
	}

	/// <summary>
	/// Writes an event identifying the resource by its cmdb.id custom property.
	/// </summary>
	[Fact]
	public async Task WriteEvent_UsingResourceCustomProperty_ShouldSucceed()
	{
		var (name, value) = GetCmdbId(await GetResourceAsync());

		WriteOneEvent(sink => sink.LogicMonitor(LogicMonitorClientOptions, name, value, FieldProperties), nameof(WriteEvent_UsingResourceCustomProperty_ShouldSucceed));
	}

	/// <summary>
	/// Writes an event identifying the resource by a dictionary of properties.
	/// </summary>
	[Fact]
	public async Task WriteEvent_UsingPropertyDictionary_ShouldSucceed()
	{
		var resource = await GetResourceAsync();
		var propertyDictionary = new Dictionary<string, string>
		{
			{ "system.deviceId", resource.Id.ToString(CultureInfo.InvariantCulture) },
			{ "system.displayname", resource.DisplayName },
			{ "cmdb.id", GetCmdbId(resource).Value }
		};

		WriteOneEvent(sink => sink.LogicMonitor(LogicMonitorClientOptions, propertyDictionary, FieldProperties), nameof(WriteEvent_UsingPropertyDictionary_ShouldSucceed));
	}
}
