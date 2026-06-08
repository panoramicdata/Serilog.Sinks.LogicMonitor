using Serilog.Sinks.LogicMonitor.IntegrationTests.Objects;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// Integration tests for writing log events to LogicMonitor.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LmWriteTests"/> class.
/// </remarks>
/// <param name="testOutputHelper">The test output helper.</param>
public class LmWriteTests(ITestOutputHelper testOutputHelper) : BaseTest(testOutputHelper)
{

	/// <summary>
	/// Verifies that fifty events can be written to LogicMonitor.
	/// </summary>
	[Fact]
	public void Write50Events_ShouldWrite50EventsToLogicMonitor()
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
		var testObj2 = new TestObjectType2 { DateProp1 = DateTime.Now, NestedProp = testObject };
		var fieldProperties = new Dictionary<string, FieldWriterBase>
			{
				{"message", new RenderedMessageFieldWriter() },
				{"message_template", new MessageTemplateFieldWriter() },
				{"level", new LevelFieldWriter(true) },
				{"raise_date", new TimestampFieldWriter() },
				{"exception", new ExceptionFieldWriter() },
				{"properties", new LogEventSerializedFieldWriter() },
				{"props_test", new PropertiesFieldWriter() },
				{"machine_name", new SinglePropertyFieldWriter("MachineName", format: "l") }
			};

		var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			ResourceId,
			fieldProperties)
			.Enrich.WithMachineName()
			.CreateLogger();

		const int rowsCount = 50;
		for (var i = 0; i < rowsCount; i++)
		{
			logger.Information("Test{testNo}: {@testObject} test2: {@testObj2} testStr: {@testStr:l}", i, testObject, testObj2, "stringValue");
		}

		logger.Dispose();

		// TODO - Check in LM that it worked.
	}

	/// <summary>
	/// Verifies that an event containing a null character in JSON can be written.
	/// </summary>
	[Fact]
	public void WriteEventWithZeroCodeCharInJson_ShouldWriteEventToLogicMonitor()
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test\\u0000" };
		var fieldProperties = new Dictionary<string, FieldWriterBase>
			{
				{"message", new RenderedMessageFieldWriter() },
				{"message_template", new MessageTemplateFieldWriter() },
				{"level", new LevelFieldWriter(true) },
				{"raise_date", new TimestampFieldWriter() },
				{"exception", new ExceptionFieldWriter() },
				{"properties", new LogEventSerializedFieldWriter() },
				{"props_test", new PropertiesFieldWriter() }
			};

		using var logger =
			new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			ResourceId,
			fieldProperties)
			.CreateLogger();
		logger.Information("Test: {@testObject} testStr: {@testStr:l}", testObject, "stringValue");

		// TODO - test it worked
	}

	/// <summary>
	/// Verifies writing with quoted column names in insert statements.
	/// </summary>
	[Fact]
	public void QuotedColumnNamesWithInsertStatements_ShouldInsertEventToDb()
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
		var fieldProperties = new Dictionary<string, FieldWriterBase>
			{
				{"message", new RenderedMessageFieldWriter() },
				{"\"message_template\"", new MessageTemplateFieldWriter() },
				{"\"level\"", new LevelFieldWriter(true) },
				{"raise_date", new TimestampFieldWriter() },
				{"exception", new ExceptionFieldWriter() },
				{"properties", new LogEventSerializedFieldWriter() },
				{"props_test", new PropertiesFieldWriter() }
			};

		using var logger =
			 new LoggerConfiguration()
			 .WriteTo
			 .LogicMonitor(
						LogicMonitorClientOptions,
						ResourceId,
						fieldProperties
						)
			 .CreateLogger();

		logger.Information("Test: {@testObject} testStr: {@testStr:l}", testObject, "stringValue");

		// TODO - check it worked
	}

	/// <summary>
	/// Verifies behavior when a single-property column writer property is missing.
	/// </summary>
	[Fact]
	public void PropertyForSinglePropertyColumnWriterDoesNotExistsWithInsertStatements_ShouldInsertEventToDb()
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
		var fieldProperties = new Dictionary<string, FieldWriterBase>
			{
				{"message", new RenderedMessageFieldWriter() },
				{"message_template", new MessageTemplateFieldWriter() },
				{"level", new LevelFieldWriter(true) },
				{"raise_date", new TimestampFieldWriter() },
				{"exception", new ExceptionFieldWriter() },
				{"properties", new LogEventSerializedFieldWriter() },
				{"props_test", new PropertiesFieldWriter() },
				{"machine_name", new SinglePropertyFieldWriter("MachineName", format: "l") }
			};

		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			ResourceId,
			fieldProperties
		)
			.CreateLogger();
		logger.Information("Test: {@testObject} testStr: {@testStr:l}", testObject, "stringValue");

		// TODO - check it worked
	}

	/// <summary>
	/// Verifies that auto-creation creates the target table.
	/// </summary>
	[Fact]
	public void AutoCreateTableIsTrue_ShouldCreateTable()
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
		var testObj2 = new TestObjectType2 { DateProp1 = DateTime.Now, NestedProp = testObject };
		var fieldProperties = new Dictionary<string, FieldWriterBase>
			{
				{"message", new RenderedMessageFieldWriter() },
				{"message_template", new MessageTemplateFieldWriter() },
				{"level", new LevelFieldWriter(true) },
				{"raise_date", new TimestampFieldWriter() },
				{"exception", new ExceptionFieldWriter() },
				{"properties", new LogEventSerializedFieldWriter() },
				{"props_test", new PropertiesFieldWriter() },
				{"int_prop_test", new SinglePropertyFieldWriter("testNo", PropertyWriteMethod.Raw) },
				{"machine_name", new SinglePropertyFieldWriter("MachineName", format: "l") }
			};
		const int rowsCount = 50;

		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			ResourceId,
			fieldProperties)
			.Enrich.WithMachineName()
			.CreateLogger();
		for (var i = 0; i < rowsCount; i++)
		{
			logger.Information("Test{testNo}: {@testObject} test2: {@testObj2} testStr: {@testStr:l}", i, testObject, testObj2, "stringValue");
		}

		// TODO - check it worked
	}

	/// <summary>
	/// Verifies table and column handling when names differ by case.
	/// </summary>
	[Fact]
	public void ColumnsAndTableWithDifferentCaseName_ShouldCreateTableAndIsertEvents()
	{
		var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
		var testObj2 = new TestObjectType2 { DateProp1 = DateTime.Now, NestedProp = testObject };
		var fieldProperties = new Dictionary<string, FieldWriterBase>
			{
				{"Message", new RenderedMessageFieldWriter() },
				{"MessageTemplate", new MessageTemplateFieldWriter() },
				{"Level", new LevelFieldWriter(true) },
				{"RaiseDate", new TimestampFieldWriter() },
				{"\"Exception\"", new ExceptionFieldWriter() },
				{"Properties", new LogEventSerializedFieldWriter() },
				{"PropsTest", new PropertiesFieldWriter() },
				{"IntPropTest", new SinglePropertyFieldWriter("testNo", PropertyWriteMethod.Raw) },
				{"MachineName", new SinglePropertyFieldWriter("MachineName", format: "l") }
			};

		const int rowsCount = 50;

		using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
			LogicMonitorClientOptions,
			ResourceId,
			fieldProperties
		)
			.Enrich.WithMachineName()
			.CreateLogger();
		for (var i = 0; i < rowsCount; i++)
		{
			logger.Information("Test{testNo}: {@testObject} test2: {@testObj2} testStr: {@testStr:l}", i, testObject, testObj2, "stringValue");
		}
		// TODO - check it worked
	}
}
