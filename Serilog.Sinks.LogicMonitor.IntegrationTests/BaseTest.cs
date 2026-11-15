using Serilog.Sinks.LogicMonitor.IntegrationTests.Objects;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Base class for the integration tests, which write to a live LogicMonitor portal configured in appsettings.json.
/// </summary>
public abstract class BaseTest
{
	private readonly TestConfig _config;

	/// <summary>
	/// Loads the test configuration and routes the client logging to the test output.
	/// </summary>
	/// <param name="testOutputHelper">The test output helper.</param>
	protected BaseTest(ITestOutputHelper testOutputHelper)
	{
		var fileInfo = new FileInfo("../../../appsettings.json");
		var config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile(fileInfo.FullName, false, true)
			.Build();
		_config = config.Get<TestConfig>() ?? throw new FormatException("Failed to load test configuration from appsettings.json");
		_config.ClientOptions.Logger = new TestOutputLogger(testOutputHelper);
	}

	/// <summary>
	/// The client options loaded from appsettings.json.
	/// </summary>
	protected LogicMonitorClientOptions LogicMonitorClientOptions => _config.ClientOptions;

	/// <summary>
	/// The default field writers.
	/// </summary>
	protected Dictionary<string, FieldWriterBase> FieldProperties { get; } = FieldSets.Default();

	/// <summary>
	/// The id of the device that events are written to. It must have the "cmdb.id" custom property.
	/// </summary>
	protected int ResourceId => _config.ResourceId;

	/// <summary>
	/// Writes events carrying nested test objects to the configured resource.
	/// </summary>
	/// <param name="fields">The field writers to use.</param>
	/// <param name="count">The number of events to write.</param>
	protected void WriteTestEvents(IDictionary<string, FieldWriterBase> fields, int count)
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
		var testObj2 = new TestObjectType2 { DateProp1 = DateTime.Now, NestedProp = testObject };

		using var logger = new LoggerConfiguration()
			.WriteTo.LogicMonitor(LogicMonitorClientOptions, ResourceId, fields)
			.Enrich.WithMachineName()
			.CreateLogger();
		for (var i = 0; i < count; i++)
		{
			logger.Information("Test{testNo}: {@testObject} test2: {@testObj2} testStr: {@testStr:l}", i, testObject, testObj2, "stringValue");
		}
	}

	/// <summary>
	/// Writes one event, identifying the device with the supplied sink configuration.
	/// </summary>
	/// <param name="configure">Adds the LogicMonitor sink to the configuration.</param>
	/// <param name="testName">The name of the calling test.</param>
	protected static void WriteOneEvent(Func<Configuration.LoggerSinkConfiguration, LoggerConfiguration> configure, string testName)
	{
		using var logger = configure(new LoggerConfiguration().WriteTo)
			.Enrich.WithMachineName()
			.CreateLogger();
		logger.Information($"Serilog.Sinks.LogicMonitor: {testName}");
	}

	/// <summary>
	/// Gets the configured resource from LogicMonitor.
	/// </summary>
	protected async Task<Resource> GetResourceAsync()
	{
		using var client = new LogicMonitorClient(LogicMonitorClientOptions);
		return await client.GetAsync<Resource>(ResourceId, default);
	}

	/// <summary>
	/// Gets the "cmdb.id" custom property of a resource.
	/// </summary>
	/// <param name="resource">The resource.</param>
	protected static (string Name, string Value) GetCmdbId(Resource resource)
	{
		var property = resource.CustomProperties.FirstOrDefault(cp => cp.Name == "cmdb.id")
			?? throw new FormatException($"Resource {resource.DisplayName} is missing the cmdb.id custom property needed for this test.");
		return (property.Name, property.Value);
	}
}
