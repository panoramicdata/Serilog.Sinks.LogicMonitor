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
		/// Adds a sink which writes to LogicMonitor using the resource id as the device identifier.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
		/// <param name="resourceId">The resource id.</param>
		/// <param name="fieldOptions">Field writers</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
		/// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration LogicMonitor(this LoggerSinkConfiguration sinkConfiguration,
			 LogicMonitorClientOptions logicmonitorClientOptions,
			 int resourceId,
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

			if (resourceId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(resourceId), "Should be greater than 0.");
			}

			period ??= DefaultPeriod;

			var lmSink = new LogicMonitorSink(logicmonitorClientOptions,
				resourceId,
				period.Value,
				formatProvider,
				fieldOptions,
				batchSizeLimit);

			return sinkConfiguration.Sink(lmSink, restrictedToMinimumLevel, levelSwitch);
		}

		/// <summary>
		/// Adds a sink which writes to LogicMonitor using the resource display name as the device identifier.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
		/// <param name="resourceDiplayName">The resource display name.</param>
		/// <param name="fieldOptions">Field writers</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
		/// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration LogicMonitor(this LoggerSinkConfiguration sinkConfiguration,
			 LogicMonitorClientOptions logicmonitorClientOptions,
			 string resourceDiplayName,
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

			if (resourceDiplayName is null)
			{
				throw new ArgumentNullException(nameof(resourceDiplayName));
			}

			period ??= DefaultPeriod;

			var lmSink = new LogicMonitorSink(logicmonitorClientOptions,
				resourceDiplayName,
				period.Value,
				formatProvider,
				fieldOptions,
				batchSizeLimit);

			return sinkConfiguration.Sink(lmSink, restrictedToMinimumLevel, levelSwitch);
		}

		/// <summary>
		/// Adds a sink which writes to LogicMonitor using the resource display name as the device identifier.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
		/// <param name="customPropertyName">The custom property name.</param>
		/// <param name="customPropertyValue">
		///		This must be unique across the specified custom property for the entire LM portal.
		///	</param>
		/// <param name="fieldOptions">Field writers</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
		/// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration LogicMonitor(this LoggerSinkConfiguration sinkConfiguration,
			 LogicMonitorClientOptions logicmonitorClientOptions,
			 string customPropertyName,
			 string customPropertyValue,
			 Dictionary<string, FieldWriterBase>? fieldOptions = null,
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

			if (customPropertyName is null)
			{
				throw new ArgumentNullException(nameof(customPropertyName));
			}

			if (customPropertyValue is null)
			{
				throw new ArgumentNullException(nameof(customPropertyValue));
			}

			period ??= DefaultPeriod;

			var lmSink = new LogicMonitorSink(logicmonitorClientOptions,
				customPropertyName,
				customPropertyValue,
				period.Value,
				formatProvider,
				fieldOptions,
				batchSizeLimit);

			return sinkConfiguration.Sink(lmSink, restrictedToMinimumLevel, levelSwitch);
		}


		/// <summary>
		/// Adds a sink which writes to LogicMonitor using the resource display name as the device identifier.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
		/// <param name="propertyDictionary">The custom property dictionary.</param>
		/// <param name="fieldOptions">Field writers</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
		/// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration LogicMonitor(this LoggerSinkConfiguration sinkConfiguration,
			 LogicMonitorClientOptions logicmonitorClientOptions,
			 Dictionary<string, string> propertyDictionary,
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

			if (propertyDictionary is null)
			{
				throw new ArgumentNullException(nameof(propertyDictionary));
			}

			period ??= DefaultPeriod;

			var lmSink = new LogicMonitorSink(logicmonitorClientOptions,
				propertyDictionary,
				period.Value,
				formatProvider,
				fieldOptions,
				batchSizeLimit);

			return sinkConfiguration.Sink(lmSink, restrictedToMinimumLevel, levelSwitch);
		}
	}
}
