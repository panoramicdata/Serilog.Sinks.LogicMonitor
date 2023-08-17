using LogicMonitor.Api;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serilog.Sinks.LogicMonitor
{
	public class LogicMonitorSink : PeriodicBatchingSink
	{
		private readonly LogicMonitorClientOptions _logicMonitorClientOptions;
		private readonly int _deviceId;
		private readonly IDictionary<string, FieldWriterBase>? _fieldOptions;
		private readonly IFormatProvider? _formatProvider;

		public const int DefaultBatchSizeLimit = 30;
		public const int DefaultQueueLimit = int.MaxValue;

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			int deviceId,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit) : base(batchSizeLimit, period)
		{
			_logicMonitorClientOptions = logicMonitorClientOptions ?? throw new ArgumentNullException(nameof(logicMonitorClientOptions));
			if (deviceId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(deviceId), "Should be greater than 0.");
			}
			_deviceId = deviceId;
			_formatProvider = formatProvider;
			_fieldOptions = fieldOptions ?? FieldOptions.Default;
		}

		public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
			int deviceId,
			TimeSpan period,
			IFormatProvider? formatProvider = null,
			IDictionary<string, FieldWriterBase>? fieldOptions = null,
			int batchSizeLimit = DefaultBatchSizeLimit,
			int queueLimit = DefaultQueueLimit) : base(batchSizeLimit, period, queueLimit)
		{
			_logicMonitorClientOptions = logicMonitorClientOptions ?? throw new ArgumentNullException(nameof(logicMonitorClientOptions));
			if (deviceId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(deviceId), "Should be greater than 0.");
			}
			_deviceId = deviceId;
			_formatProvider = formatProvider;
			_fieldOptions = fieldOptions ?? FieldOptions.Default;
		}

		protected override async Task EmitBatchAsync(IEnumerable<LogEvent> logEventBatch)
		{
			using var logicMonitorClient = new LogicMonitorClient(_logicMonitorClientOptions);
			foreach (var logEvent in logEventBatch)
			{
				// Write to LogicMonitor
				_ = await logicMonitorClient
					.WriteLogAsync(_deviceId, logEvent.RenderMessage(_formatProvider))
					.ConfigureAwait(false);
			}
		}
	}
}