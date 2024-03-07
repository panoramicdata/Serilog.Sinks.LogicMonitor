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
		private readonly int _deviceId;
		private readonly TimeSpan _period;
		private readonly IDictionary<string, FieldWriterBase>? _fieldOptions;
		private readonly int _batchSizeLimit;
		private readonly int _queueLimit;
		private readonly IFormatProvider? _formatProvider;

		public const int DefaultBatchSizeLimit = 30;
		public const int DefaultQueueLimit = int.MaxValue;

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			int deviceId,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit)
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
		}

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
		}

		public async Task EmitBatchAsync(IEnumerable<LogEvent> logEventBatch)
		{
			using var logicMonitorClient = new LogicMonitorClient(_logicMonitorClientOptions);
			// Write to LogicMonitor
			_ = await logicMonitorClient
				.WriteLogAsync(
					logEventBatch.Select(logEvent => new WriteLogRequest(
						logEvent.Level switch
						{
							LogEventLevel.Fatal => WriteLogLevel.Error,
							LogEventLevel.Error => WriteLogLevel.Error,
							LogEventLevel.Warning => WriteLogLevel.Warning,
							LogEventLevel.Information => WriteLogLevel.Info,
							LogEventLevel.Debug => WriteLogLevel.Debug,
							LogEventLevel.Verbose => WriteLogLevel.Debug,
							_ => WriteLogLevel.Info
						},
						_deviceId,
						logEvent.RenderMessage(_formatProvider))),
					cancellationToken: default
				)
				.ConfigureAwait(false);
		}

		public Task OnEmptyBatchAsync() => Task.CompletedTask;

		public void Emit(LogEvent logEvent)
		{

		}
	}
}