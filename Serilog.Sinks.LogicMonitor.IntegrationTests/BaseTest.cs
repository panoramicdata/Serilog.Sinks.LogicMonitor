using LogicMonitor.Api;
using Microsoft.Extensions.Configuration;
using System;
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

		protected int DeviceId => _config.DeviceId;
	}
}