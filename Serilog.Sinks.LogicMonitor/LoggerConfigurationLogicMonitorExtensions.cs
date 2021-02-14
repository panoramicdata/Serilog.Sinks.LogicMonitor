using LogicMonitor.Api;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace Serilog.Sinks.LogicMonitor
{
	public static class LoggerConfigurationLogicMonitorExtensions
	{
		/// <summary>
		/// Default time to wait between checking for event batches.
		/// </summary>
		public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(5);

		/// <summary>
		/// Adds a sink which writes to LogicMonitor
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
		/// <param name="fieldOptions">Field writers</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
		/// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration LogicMonitor(this LoggerSinkConfiguration sinkConfiguration,
			 LogicMonitorClientOptions logicmonitorClientOptions,
			 int deviceId,
			 IDictionary<string, FieldWriterBase>? fieldOptions = null,
			 LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
			 TimeSpan? period = null,
			 IFormatProvider? formatProvider = null,
			 int batchSizeLimit = LogicMonitorSink.DefaultBatchSizeLimit,
			 LoggingLevelSwitch? levelSwitch = null
			)
		{
			if (sinkConfiguration is null)
			{
				throw new ArgumentNullException(nameof(sinkConfiguration));
			}
			if (logicmonitorClientOptions is null)
			{
				throw new ArgumentNullException(nameof(logicmonitorClientOptions));
			}
			if (deviceId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(deviceId), "Should be greater than 0.");
			}

			period ??= DefaultPeriod;

			var lmSink = new LogicMonitorSink(logicmonitorClientOptions,
				deviceId,
				period.Value,
				formatProvider,
				fieldOptions,
				batchSizeLimit);

			return sinkConfiguration.Sink(lmSink, restrictedToMinimumLevel, levelSwitch);
		}
	}
}
