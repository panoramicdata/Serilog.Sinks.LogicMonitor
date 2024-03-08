using LogicMonitor.Api;
using LogicMonitor.Api.Devices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests
{
	public class LmWriteWithDifferentMethodTests(ITestOutputHelper testOutputHelper) : BaseTest(testOutputHelper)
	{
		[Fact]
		public void WriteEvent_UsingResourceId_ShouldSucceed()
		{
			using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
				LogicMonitorClientOptions,
				DeviceId,
				FieldProperties
			)
				.Enrich
				.WithMachineName()
				.CreateLogger();

			logger.Information($"Serilog.Sinks.LogicMonitor: {nameof(WriteEvent_UsingResourceId_ShouldSucceed)}");

			// TODO - Test it worked
		}

		[Fact]
		public async void WriteEvent_UsingResourceDisplayName_ShouldSucceed()
		{
			var logicMonitorClient = new LogicMonitorClient(LogicMonitorClientOptions);
			var device = await logicMonitorClient.GetAsync<Device>(DeviceId, default);

			using var logger = new LoggerConfiguration().WriteTo.LogicMonitor(
				LogicMonitorClientOptions,
				device.DisplayName,
				FieldProperties
			)
				.Enrich
				.WithMachineName()
				.CreateLogger();

			logger.Information($"Serilog.Sinks.LogicMonitor: {nameof(WriteEvent_UsingResourceDisplayName_ShouldSucceed)}");

			// TODO - Test it worked
		}

		[Fact]
		public async void WriteEvent_UsingResourceCustomProperty_ShouldSucceed()
		{
			var logicMonitorClient = new LogicMonitorClient(LogicMonitorClientOptions);
			var device = await logicMonitorClient.GetAsync<Device>(DeviceId, default);

			// Get the "cmdb.id" custom property from the device
			var customProperty = device
				.CustomProperties
				.FirstOrDefault(cp => cp.Name == "cmdb.id")
				?? throw new FormatException($"Resource {device.DisplayName} is missing the cmdb.id custom property needed for this unit test.");

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

		[Fact]
		public async void WriteEvent_UsingPropertyDictionary_ShouldSucceed()
		{
			var logicMonitorClient = new LogicMonitorClient(LogicMonitorClientOptions);
			var device = await logicMonitorClient.GetAsync<Device>(DeviceId, default);

			// Get the "cmdb.id" custom property from the device
			var customProperty = device
				.CustomProperties
				.FirstOrDefault(cp => cp.Name == "cmdb.id");

			var propertyDictionary = new Dictionary<string, string>
			{
				{ "system.deviceId", device.Id.ToString(CultureInfo.InvariantCulture) },
				{ "system.displayname", device.DisplayName },
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
}