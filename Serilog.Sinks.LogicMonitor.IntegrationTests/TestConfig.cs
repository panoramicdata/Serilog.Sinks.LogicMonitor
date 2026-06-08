namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Represents the configuration for the LogicMonitor sink integration tests. This class is used to load the necessary settings from the appsettings.json file, including the LogicMonitor client options and the resource ID of the target device in LogicMonitor that the tests will write events to. The client options include a logger that writes to the test output, allowing for visibility of log messages during test runs. The resource ID is used to identify the specific device in LogicMonitor that the test events will be associated with, and must correspond to an existing device in the LogicMonitor account that has the appropriate custom properties configured for the tests to work correctly.
/// </summary>
public class TestConfig
{
	/// <summary>
	/// The LogicMonitorClientOptions to use for the tests. This is populated from the appsettings.json file, and includes a logger that writes to the test output for visibility during test runs. The client options also include any necessary settings for connecting to the LogicMonitor API, such as account information and credentials, as well as any additional options needed for the tests. If the configuration cannot be loaded or is missing required settings, a FormatException will be thrown to indicate that the test setup is invalid.
	/// </summary>
	public required LogicMonitorClientOptions ClientOptions { get; set; }

	/// <summary>
	/// The resource ID of the target device in LogicMonitor that the tests will write events to. This must correspond to an existing device in the LogicMonitor account that has the appropriate custom properties configured for the tests to work correctly.
	/// </summary>
	public required int ResourceId { get; set; }
}