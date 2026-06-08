namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Base test class for LogicMonitor sink integration tests. This class is responsible for loading the test configuration from the appsettings.json file and providing common properties and setup for the derived test classes. The configuration includes the LogicMonitor client options, which are used to create instances of the LogicMonitorClient for interacting with the LogicMonitor API during the tests. The client options also include a logger that writes to the test output, allowing for visibility of log messages during test runs. Additionally, this class defines a set of field properties that determine how log event fields are written to LogicMonitor, as well as a property for the resource ID of the target device in LogicMonitor that the tests will write events to.
/// </summary>
public abstract class BaseTest
{
	private readonly TestConfig _config;

	/// <summary>
	/// Constructor for the BaseTest class. This constructor loads the test configuration from the appsettings.json file and initializes the LogicMonitor client options with a logger that writes to the test output. The configuration is expected to include the necessary settings for connecting to the LogicMonitor API, such as account information and credentials, as well as any additional options needed for the tests. If the configuration cannot be loaded or is missing required settings, a FormatException will be thrown to indicate that the test setup is invalid.
	/// </summary>
	/// <param name="testOutputHelper"></param>
	protected BaseTest(ITestOutputHelper testOutputHelper)
	{
		var fileInfo = new FileInfo("../../../appsettings.json");
		var config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile(fileInfo.FullName, false, true)
			.Build();
		_config = config.Get<TestConfig>() ?? throw new FormatException("Failed to load test configuration from appsettings.json");
		_config.ClientOptions.Logger = testOutputHelper.BuildLogger();
	}

	/// <summary>
	/// The LogicMonitorClientOptions to use for the tests. This is populated from the appsettings.json file, and includes a logger that writes to the test output for visibility during test runs.
	/// </summary>
	protected LogicMonitorClientOptions LogicMonitorClientOptions => _config.ClientOptions;

	/// <summary>
	/// The field properties to use for the tests. This is a dictionary of field names to FieldWriterBase instances that determine how the fields are written to LogicMonitor. The default set of fields includes the rendered message, message template, level, raise date, exception, properties, and machine name.
	/// </summary>
	protected Dictionary<string, FieldWriterBase> FieldProperties { get; } = new Dictionary<string, FieldWriterBase>
	{
		{"message", new RenderedMessageFieldWriter() },
		{"message_template", new MessageTemplateFieldWriter() },
		{"level", new LevelFieldWriter(true) },
		{"raise_date", new TimestampFieldWriter() },
		{"exception", new ExceptionFieldWriter() },
		{"properties", new LogEventSerializedFieldWriter() },
		{"props_test", new PropertiesFieldWriter() },
		{"machine_name", new SinglePropertyFieldWriter("MachineName") }
	};

	/// <summary>
	/// The ID of the device resource in LogicMonitor that the test events will be written to. This device must exist and be configured with the "cmdb.id" custom property for the tests to work.
	/// </summary>
	protected int ResourceId => _config.ResourceId;
}