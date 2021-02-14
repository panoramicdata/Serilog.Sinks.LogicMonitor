using Serilog.Sinks.LogicMonitor.IntegrationTests.Objects;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests
{
	public class LmWriteWithSchemaTests : BaseTest
	{
		public LmWriteWithSchemaTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void Write50Events_ShouldInsert50EventsToDb()
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
					{"machine_name", new SinglePropertyFieldWriter("MachineName") }
				};

			using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
				LogicMonitorClientOptions,
				DeviceId,
				fieldProperties
			)
				.Enrich.WithMachineName()
				.CreateLogger();
			for (var i = 0; i < 50; i++)
			{
				logger.Information("Test{testNo}: {@testObject} test2: {@testObj2}", i, testObject, testObj2);
			}

			// TODO - Test it worked
		}

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
					{"int_prop_test", new SinglePropertyFieldWriter("testNo", format: "l") },
					{"machine_name", new SinglePropertyFieldWriter("MachineName", format: "l") }
				};

			var logger =
				new LoggerConfiguration().WriteTo.LogicMonitor(
					LogicMonitorClientOptions,
					DeviceId,
					fieldProperties)
				.Enrich.WithMachineName()
				.CreateLogger();

			const int rowsCount = 50;
			for (var i = 0; i < rowsCount; i++)
			{
				logger.Information("Test{testNo}: {@testObject} test2: {@testObj2} testStr: {@testStr:l}", i, testObject, testObj2, "stringValue");
			}

			logger.Dispose();

			// TODO - Check is actually worked
		}

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
				DeviceId,
				fieldProperties
			)
				.Enrich.WithMachineName()
				.CreateLogger();
			for (var i = 0; i < rowsCount; i++)
			{
				logger.Information("Test{testNo}: {@testObject} test2: {@testObj2} testStr: {@testStr:l}", i, testObject, testObj2, "stringValue");
			}

			// TODO - Test this works
		}
	}
}