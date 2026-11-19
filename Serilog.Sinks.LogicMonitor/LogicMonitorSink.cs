using LogicMonitor.Api;
using LogicMonitor.Api.Logging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Runtime.InteropServices;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Serilog batched sink implementation that writes events to LogicMonitor LM Logs.
/// </summary>
public class LogicMonitorSink : IBatchedLogEventSink, IDisposable
{
	private readonly Func<WriteLogLevel, string, WriteLogRequest> _buildRequest;
	private readonly IDictionary<string, FieldWriterBase> _fieldOptions;
	private readonly IFormatProvider? _formatProvider;
	private IWriteLogClient _client;
	private bool disposedValue;

	/// <summary>
	/// Default maximum number of events to include in a single batch.
	/// </summary>
	public const int DefaultBatchSizeLimit = 30;

	/// <summary>
	/// Default maximum number of queued events awaiting batching.
	/// </summary>
	public const int DefaultQueueLimit = int.MaxValue;

	/// <summary>
	/// Initializes a new sink that identifies the target device by resource ID.
	/// </summary>
	/// <param name="logicMonitorClientOptions">LogicMonitor API client options.</param>
	/// <param name="deviceId">The LogicMonitor resource ID.</param>
	/// <param name="formatProvider">Optional format provider for rendered messages.</param>
	/// <param name="fieldOptions">Optional field writer overrides.</param>
	public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
		int deviceId,
		[Optional] [DefaultParameterValue(null)] IFormatProvider? formatProvider,
		[Optional] [DefaultParameterValue(null)] IDictionary<string, FieldWriterBase>? fieldOptions
		) : this(
			logicMonitorClientOptions,
			(level, message) => new WriteLogRequest(level, deviceId, message),
			formatProvider,
			fieldOptions)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deviceId);
	}

	/// <summary>
	/// Initializes a new sink that identifies the target device by display name.
	/// </summary>
	/// <param name="logicMonitorClientOptions">LogicMonitor API client options.</param>
	/// <param name="resourceDisplayName">The LogicMonitor resource display name.</param>
	/// <param name="formatProvider">Optional format provider for rendered messages.</param>
	/// <param name="fieldOptions">Optional field writer overrides.</param>
	public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
		string resourceDisplayName,
		[Optional] [DefaultParameterValue(null)] IFormatProvider? formatProvider,
		[Optional] [DefaultParameterValue(null)] IDictionary<string, FieldWriterBase>? fieldOptions
		) : this(
			logicMonitorClientOptions,
			(level, message) => new WriteLogRequest(level, resourceDisplayName, message),
			formatProvider,
			fieldOptions)
	{
	}

	/// <summary>
	/// Initializes a new sink that identifies the target device by a custom property name/value pair.
	/// </summary>
	/// <param name="logicMonitorClientOptions">LogicMonitor API client options.</param>
	/// <param name="customPropertyName">The custom property name.</param>
	/// <param name="customPropertyValue">The custom property value.</param>
	/// <param name="formatProvider">Optional format provider for rendered messages.</param>
	/// <param name="fieldOptions">Optional field writer overrides.</param>
	public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
		string customPropertyName,
		string customPropertyValue,
		[Optional] [DefaultParameterValue(null)] IFormatProvider? formatProvider,
		[Optional] [DefaultParameterValue(null)] IDictionary<string, FieldWriterBase>? fieldOptions
		) : this(
			logicMonitorClientOptions,
			(level, message) => new WriteLogRequest(level, customPropertyName, customPropertyValue, message),
			formatProvider,
			fieldOptions)
	{
	}

	/// <summary>
	/// Initializes a new sink that identifies the target device by a custom property dictionary.
	/// </summary>
	/// <param name="logicMonitorClientOptions">LogicMonitor API client options.</param>
	/// <param name="propertyDictionary">The custom property dictionary used for device identification.</param>
	/// <param name="formatProvider">Optional format provider for rendered messages.</param>
	/// <param name="fieldOptions">Optional field writer overrides.</param>
	public LogicMonitorSink(LogicMonitorClientOptions logicMonitorClientOptions,
		Dictionary<string, string> propertyDictionary,
		[Optional] [DefaultParameterValue(null)] IFormatProvider? formatProvider,
		[Optional] [DefaultParameterValue(null)] IDictionary<string, FieldWriterBase>? fieldOptions
		) : this(
			logicMonitorClientOptions,
			(level, message) => new WriteLogRequest(level, propertyDictionary, message),
			formatProvider,
			fieldOptions)
	{
		ArgumentNullException.ThrowIfNull(propertyDictionary);
	}

	private LogicMonitorSink(
		LogicMonitorClientOptions logicMonitorClientOptions,
		Func<WriteLogLevel, string, WriteLogRequest> buildRequest,
		IFormatProvider? formatProvider,
		IDictionary<string, FieldWriterBase>? fieldOptions)
	{
		ArgumentNullException.ThrowIfNull(logicMonitorClientOptions);

		_buildRequest = buildRequest;
		_formatProvider = formatProvider;
		_fieldOptions = fieldOptions ?? FieldOptions.Default;
		_client = new LogicMonitorWriteLogClient(logicMonitorClientOptions);
	}

	/// <summary>
	/// Emits a batch of log events to LogicMonitor.
	/// </summary>
	/// <param name="logEventBatch">The batch of log events to write.</param>
	public async Task EmitBatchAsync(IEnumerable<LogEvent> logEventBatch)
		=> await _client
			.WriteLogAsync(
				logEventBatch.Select(logEvent => _buildRequest(GetWriteLogLevel(logEvent), logEvent.RenderMessage(_formatProvider))),
				default)
			.ConfigureAwait(false);

	internal IDictionary<string, FieldWriterBase> FieldOptionsInUse => _fieldOptions;

	internal void ReplaceClient(IWriteLogClient client)
	{
		_client.Dispose();
		_client = client;
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

	/// <summary>
	/// Called when a batching cycle has no events.
	/// </summary>
	/// <returns>A completed task.</returns>
	public Task OnEmptyBatchAsync() => Task.CompletedTask;

	/// <summary>
	/// Releases managed and unmanaged resources.
	/// </summary>
	/// <param name="disposing"><see langword="true"/> to dispose managed resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_client.Dispose();
			}

			disposedValue = true;
		}
	}

	/// <summary>
	/// Releases sink resources.
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
