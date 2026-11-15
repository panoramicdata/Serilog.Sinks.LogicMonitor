using LogicMonitor.Api;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Extension methods for wiring LogicMonitor sinks into Serilog configuration.
/// </summary>
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
	/// <param name="queueLimit">The maximum number of events retained in the queue before backpressure is applied.</param>
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
		ArgumentNullException.ThrowIfNull(sinkConfiguration);
		ArgumentNullException.ThrowIfNull(logicmonitorClientOptions);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(resourceId);

		var lmSink = new LogicMonitorSink(logicmonitorClientOptions, resourceId, formatProvider, fieldOptions);
		return AddBatchingSink(sinkConfiguration, lmSink, restrictedToMinimumLevel, period, batchSizeLimit, queueLimit, levelSwitch);
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
	/// <param name="queueLimit">The maximum number of events retained in the queue before backpressure is applied.</param>
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
		ArgumentNullException.ThrowIfNull(sinkConfiguration);
		ArgumentNullException.ThrowIfNull(logicmonitorClientOptions);
		ArgumentNullException.ThrowIfNull(resourceDisplayName);

		resourceDisplayName = resourceDisplayName.Replace("{EnvironmentVariable:HOSTNAME}", Environment.GetEnvironmentVariable("HOSTNAME"));

		var lmSink = new LogicMonitorSink(logicmonitorClientOptions, resourceDisplayName, formatProvider, fieldOptions);
		return AddBatchingSink(sinkConfiguration, lmSink, restrictedToMinimumLevel, period, batchSizeLimit, queueLimit, levelSwitch);
	}

	/// <summary>
	/// Adds a sink which writes to LogicMonitor using a custom property name and value as the device identifier.
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
	/// <param name="queueLimit">The maximum number of events retained in the queue before backpressure is applied.</param>
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
		ArgumentNullException.ThrowIfNull(sinkConfiguration);
		ArgumentNullException.ThrowIfNull(logicmonitorClientOptions);
		ArgumentNullException.ThrowIfNull(customPropertyName);
		ArgumentNullException.ThrowIfNull(customPropertyValue);

		var lmSink = new LogicMonitorSink(logicmonitorClientOptions, customPropertyName, customPropertyValue, formatProvider, fieldOptions);
		return AddBatchingSink(sinkConfiguration, lmSink, restrictedToMinimumLevel, period, batchSizeLimit, queueLimit, levelSwitch);
	}

	/// <summary>
	/// Adds a sink which writes to LogicMonitor using a custom property dictionary as the device identifier.
	/// </summary>
	/// <param name="sinkConfiguration">The logger configuration.</param>
	/// <param name="logicmonitorClientOptions">The LogicMonitor client options.</param>
	/// <param name="propertyDictionary">The custom property dictionary.</param>
	/// <param name="fieldOptions">Field writers</param>
	/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
	/// <param name="period">The time to wait between checking for event batches.</param>
	/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
	/// <param name="batchSizeLimit">The maximum number of events to include to single batch.</param>
	/// <param name="queueLimit">The maximum number of events retained in the queue before backpressure is applied.</param>
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
		ArgumentNullException.ThrowIfNull(sinkConfiguration);
		ArgumentNullException.ThrowIfNull(logicmonitorClientOptions);
		ArgumentNullException.ThrowIfNull(propertyDictionary);

		var lmSink = new LogicMonitorSink(logicmonitorClientOptions, propertyDictionary, formatProvider, fieldOptions);
		return AddBatchingSink(sinkConfiguration, lmSink, restrictedToMinimumLevel, period, batchSizeLimit, queueLimit, levelSwitch);
	}

	private static LoggerConfiguration AddBatchingSink(
		LoggerSinkConfiguration sinkConfiguration,
		LogicMonitorSink lmSink,
		LogEventLevel restrictedToMinimumLevel,
		TimeSpan? period,
		int batchSizeLimit,
		int queueLimit,
		LoggingLevelSwitch? levelSwitch)
	{
		var batchingOptions = new PeriodicBatchingSinkOptions
		{
			BatchSizeLimit = batchSizeLimit,
			Period = period ?? DefaultPeriod,
			EagerlyEmitFirstEvent = true,
			QueueLimit = queueLimit
		};
		var batchingSink = new PeriodicBatchingSink(lmSink, batchingOptions);
		return sinkConfiguration.Sink(batchingSink, restrictedToMinimumLevel, levelSwitch);
	}
}
