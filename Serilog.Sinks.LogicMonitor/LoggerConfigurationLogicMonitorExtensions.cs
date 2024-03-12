using LogicMonitor.Api;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
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
			 int queueLimit = LogicMonitorSink.DefaultQueueLimit,
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

			;

			var batchingOptions = new PeriodicBatchingSinkOptions
			{
				BatchSizeLimit = batchSizeLimit,
				Period = period ?? DefaultPeriod,
				EagerlyEmitFirstEvent = true,
				QueueLimit = queueLimit
			};
			var lmSink = new LogicMonitorSink(
				logicmonitorClientOptions,
				resourceId,
				formatProvider: formatProvider,
				fieldOptions: fieldOptions);
			var batchingSink = new PeriodicBatchingSink(lmSink, batchingOptions);
			return sinkConfiguration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
		}

		/// <summary>
		/// Adds a sink which writes to LogicMonitor using the resource display name as the device identifier.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
		/// <param name="resourceDisplayName">The resource display name.</param>
		/// <param name="fieldOptions">Field writers</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
		/// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration LogicMonitor(this LoggerSinkConfiguration sinkConfiguration,
			 LogicMonitorClientOptions logicmonitorClientOptions,
			 string resourceDisplayName,
			 IDictionary<string, FieldWriterBase>? fieldOptions = null,
			 LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
			 TimeSpan? period = null,
			 IFormatProvider? formatProvider = null,
			 int batchSizeLimit = LogicMonitorSink.DefaultBatchSizeLimit,
			 int queueLimit = LogicMonitorSink.DefaultQueueLimit,
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

			if (resourceDisplayName is null)
			{
				throw new ArgumentNullException(nameof(resourceDisplayName));
			}

			// Swap out the hostname
			foreach (var env in new string[] { "HOSTNAME" })
			{
				resourceDisplayName = resourceDisplayName.Replace($"{{EnvironmentVariable:{env}}}", Environment.GetEnvironmentVariable(env));
			}

			var batchingOptions = new PeriodicBatchingSinkOptions
			{
				BatchSizeLimit = batchSizeLimit,
				Period = period ?? DefaultPeriod,
				EagerlyEmitFirstEvent = true,
				QueueLimit = queueLimit
			};
			var lmSink = new LogicMonitorSink(
				logicmonitorClientOptions,
				resourceDisplayName,
				formatProvider: formatProvider,
				fieldOptions: fieldOptions);
			var batchingSink = new PeriodicBatchingSink(lmSink, batchingOptions);
			return sinkConfiguration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
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
			 int queueLimit = LogicMonitorSink.DefaultQueueLimit,
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

			var batchingOptions = new PeriodicBatchingSinkOptions
			{
				BatchSizeLimit = batchSizeLimit,
				Period = period ?? DefaultPeriod,
				EagerlyEmitFirstEvent = true,
				QueueLimit = queueLimit
			};
			var lmSink = new LogicMonitorSink(
				logicmonitorClientOptions,
				customPropertyName,
				customPropertyValue,
				formatProvider: formatProvider,
				fieldOptions: fieldOptions);
			var batchingSink = new PeriodicBatchingSink(lmSink, batchingOptions);
			return sinkConfiguration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
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
			 int queueLimit = LogicMonitorSink.DefaultQueueLimit,
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

			var batchingOptions = new PeriodicBatchingSinkOptions
			{
				BatchSizeLimit = batchSizeLimit,
				Period = period ?? DefaultPeriod,
				EagerlyEmitFirstEvent = true,
				QueueLimit = queueLimit
			};
			var lmSink = new LogicMonitorSink(logicmonitorClientOptions, propertyDictionary, formatProvider: formatProvider, fieldOptions: fieldOptions);
			var batchingSink = new PeriodicBatchingSink(lmSink, batchingOptions);
			return sinkConfiguration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
		}
	}
}
