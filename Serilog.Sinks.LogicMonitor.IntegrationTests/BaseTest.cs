using LogicMonitor.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit.Abstractions;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests
{
	public abstract class BaseTest
	{
		private readonly TestConfig _config = new();

		protected BaseTest(ITestOutputHelper testOutputHelper)
		{
			var fileInfo = new FileInfo("../../../appsettings.json");
			var config = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile(fileInfo.FullName, false, true)
				.Build();
			_config = config.Get<TestConfig>();
			_config.ClientOptions.Logger = testOutputHelper.BuildLogger();
		}

		protected LogicMonitorClientOptions LogicMonitorClientOptions => _config.ClientOptions;

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

		protected int DeviceId => _config.DeviceId;
	}
}