using LogicMonitor.Api;
using LogicMonitor.Api.Logging;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog.Sinks.LogicMonitor
{
	public class LogicMonitorSink : IBatchedLogEventSink, ILogEventSink
	{
		private readonly LogicMonitorClientOptions _logicMonitorClientOptions;
		private readonly Dictionary<string, string>? _propertyDictionary;
		private readonly string? _customPropertyName;
		private readonly string? _customPropertyValue;
		private readonly string? _deviceDisplayName;
		private readonly int? _deviceId;
		private readonly TimeSpan _period;
		private readonly IDictionary<string, FieldWriterBase>? _fieldOptions;
		private readonly int _batchSizeLimit;
		private readonly int _queueLimit;
		private readonly WriteMethod _writeMethod;
		private readonly IFormatProvider? _formatProvider;

		public const int DefaultBatchSizeLimit = 30;
		public const int DefaultQueueLimit = int.MaxValue;

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			int deviceId,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit,
			int queueLimit = DefaultQueueLimit)
		{
			_logicMonitorClientOptions = logicMonitorClientOptions ?? throw new ArgumentNullException(nameof(logicMonitorClientOptions));
			if (deviceId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(deviceId), "Should be greater than 0.");
			}

			_deviceId = deviceId;
			_period = period;
			_formatProvider = formatProvider;
			_fieldOptions = fieldOptions ?? FieldOptions.Default;
			_batchSizeLimit = batchSizeLimit;
			_queueLimit = queueLimit;
			_writeMethod = WriteMethod.DeviceId;
		}

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			string deviceDisplayName,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit,
			int queueLimit = DefaultQueueLimit)
		{
			_logicMonitorClientOptions = logicMonitorClientOptions
				?? throw new ArgumentNullException(nameof(logicMonitorClientOptions));

			_deviceDisplayName = deviceDisplayName;
			_period = period;
			_formatProvider = formatProvider;
			_fieldOptions = fieldOptions ?? FieldOptions.Default;
			_batchSizeLimit = batchSizeLimit;
			_queueLimit = queueLimit;
			_writeMethod = WriteMethod.DeviceDisplayName;
		}

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			string customPropertyName,
			string customPropertyValue,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit,
			int queueLimit = DefaultQueueLimit)
		{
			_logicMonitorClientOptions = logicMonitorClientOptions
				?? throw new ArgumentNullException(nameof(logicMonitorClientOptions));
			_customPropertyName = customPropertyName;
			_customPropertyValue = customPropertyValue;
			_period = period;
			_formatProvider = formatProvider;
			_fieldOptions = fieldOptions ?? FieldOptions.Default;
			_batchSizeLimit = batchSizeLimit;
			_queueLimit = queueLimit;
			_writeMethod = WriteMethod.CustomProperty;
		}

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			Dictionary<string, string> propertyDictionary,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit,
			int queueLimit = DefaultQueueLimit)
		{
			_logicMonitorClientOptions = logicMonitorClientOptions
				?? throw new ArgumentNullException(nameof(logicMonitorClientOptions));
			_propertyDictionary = propertyDictionary
				?? throw new ArgumentNullException(nameof(propertyDictionary));
			_period = period;
			_formatProvider = formatProvider;
			_fieldOptions = fieldOptions ?? FieldOptions.Default;
			_batchSizeLimit = batchSizeLimit;
			_queueLimit = queueLimit;
			_writeMethod = WriteMethod.PropertyDictionary;
		}

		public async Task EmitBatchAsync(IEnumerable<LogEvent> logEventBatch)
		{
			using var logicMonitorClient = new LogicMonitorClient(_logicMonitorClientOptions);
			// Write to LogicMonitor

			_ = _writeMethod switch
			{
				WriteMethod.DeviceId => await logicMonitorClient
					.WriteLogAsync(
						logEventBatch.Select(logEvent => new WriteLogRequest(
								GetWriteLogLevel(logEvent),
								_deviceId ?? throw new InvalidOperationException($"{nameof(_deviceId)} should not be null for write method {_writeMethod}"),
								logEvent.RenderMessage(_formatProvider)
							)
						),
						cancellationToken: default
				).ConfigureAwait(false),
				WriteMethod.DeviceDisplayName => await logicMonitorClient
					.WriteLogAsync(
						logEventBatch.Select(logEvent => new WriteLogRequest(
						GetWriteLogLevel(logEvent),
						_deviceDisplayName ?? throw new InvalidOperationException($"{nameof(_deviceDisplayName)} should not be null for write method {_writeMethod}"),
						logEvent.RenderMessage(_formatProvider)
					)
				),
				cancellationToken: default
				).ConfigureAwait(false),
				WriteMethod.CustomProperty => await logicMonitorClient
					.WriteLogAsync(
						logEventBatch.Select(logEvent => new WriteLogRequest(
						GetWriteLogLevel(logEvent),
						_customPropertyName ?? throw new InvalidOperationException($"{nameof(_customPropertyName)} should not be null for write method {_writeMethod}"),
						_customPropertyValue ?? throw new InvalidOperationException($"{nameof(_customPropertyValue)} should not be null for write method {_writeMethod}"),
						logEvent.RenderMessage(_formatProvider)
					)),
					cancellationToken: default
				).ConfigureAwait(false),
				WriteMethod.PropertyDictionary => await logicMonitorClient
					.WriteLogAsync(
						logEventBatch.Select(logEvent => new WriteLogRequest(
							GetWriteLogLevel(logEvent),
							_propertyDictionary ?? throw new InvalidOperationException($"{nameof(_propertyDictionary)} should not be null for write method {_writeMethod}"),
							logEvent.RenderMessage(_formatProvider)
						)),
						cancellationToken: default
					).ConfigureAwait(false),
				_ => throw new NotSupportedException($"Unexpected write method {_writeMethod}"),
			};
		}

		private static WriteLogLevel GetWriteLogLevel(LogEvent logEvent) => logEvent.Level switch
		{
			LogEventLevel.Fatal => WriteLogLevel.Error,
			LogEventLevel.Error => WriteLogLevel.Error,
			LogEventLevel.Warning => WriteLogLevel.Warning,
			LogEventLevel.Information => WriteLogLevel.Info,
			LogEventLevel.Debug => WriteLogLevel.Debug,
			LogEventLevel.Verbose => WriteLogLevel.Debug,
			_ => WriteLogLevel.Info
		};

		public Task OnEmptyBatchAsync() => Task.CompletedTask;

		public void Emit(LogEvent logEvent) => EmitBatchAsync([logEvent]).GetAwaiter().GetResult();

		private enum WriteMethod
		{
			DeviceId,
			DeviceDisplayName,
			CustomProperty,
			PropertyDictionary
		}
	}
}