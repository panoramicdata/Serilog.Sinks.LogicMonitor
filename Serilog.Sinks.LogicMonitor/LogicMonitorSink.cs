using LogicMonitor.Api;
using LogicMonitor.Api.Logging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.LogicMonitor;

/// <summary>
/// Serilog batched sink implementation that writes events to LogicMonitor LM Logs.
/// </summary>
public class LogicMonitorSink : IBatchedLogEventSink, IDisposable
{
	private readonly Dictionary<string, string>? _propertyDictionary;
	private readonly string? _customPropertyName;
	private readonly string? _customPropertyValue;
	private readonly string? _deviceDisplayName;
	private readonly int? _deviceId;
	private readonly IDictionary<string, FieldWriterBase>? _fieldOptions;
	private readonly LogicMonitorClient _logicMonitorClient;
	private readonly WriteMethod _writeMethod;
	private readonly IFormatProvider? _formatProvider;
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
		IFormatProvider? formatProvider = null,
		IDictionary<string, FieldWriterBase>? fieldOptions = null
		) : this(logicMonitorClientOptions,
			formatProvider,
			fieldOptions
		)
	{
		if (deviceId <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(deviceId), "Should be greater than 0.");
		}

		_deviceId = deviceId;
		_writeMethod = WriteMethod.DeviceId;
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
		IFormatProvider? formatProvider = null,
		IDictionary<string, FieldWriterBase>? fieldOptions = null) : this(logicMonitorClientOptions,
			formatProvider,
			fieldOptions
		)
	{
		_deviceDisplayName = resourceDisplayName;
		_writeMethod = WriteMethod.DeviceDisplayName;
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
		IFormatProvider? formatProvider = null,
		IDictionary<string, FieldWriterBase>? fieldOptions = null
) : this(logicMonitorClientOptions,
			formatProvider,
			fieldOptions
		)
	{
		_customPropertyName = customPropertyName;
		_customPropertyValue = customPropertyValue;
		_writeMethod = WriteMethod.CustomProperty;
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
		IFormatProvider? formatProvider = null,
		IDictionary<string, FieldWriterBase>? fieldOptions = null
) : this(logicMonitorClientOptions,
			formatProvider,
			fieldOptions
		)
	{
		_propertyDictionary = propertyDictionary ?? throw new ArgumentNullException(nameof(propertyDictionary));
		_writeMethod = WriteMethod.PropertyDictionary;
	}

	private LogicMonitorSink(
		LogicMonitorClientOptions logicMonitorClientOptions,
		IFormatProvider? formatProvider,
		IDictionary<string, FieldWriterBase>? fieldOptions)
	{
		if (logicMonitorClientOptions is null)
		{
			throw new ArgumentNullException(nameof(logicMonitorClientOptions));
		}

		_formatProvider = formatProvider;
		_fieldOptions = fieldOptions ?? FieldOptions.Default;
		_logicMonitorClient = new LogicMonitorClient(logicMonitorClientOptions);
	}

	/// <summary>
	/// Emits a batch of log events to LogicMonitor.
	/// </summary>
	/// <param name="logEventBatch">The batch of log events to write.</param>
	public async Task EmitBatchAsync(IEnumerable<LogEvent> logEventBatch)
	{
		_ = _writeMethod switch
		{
			WriteMethod.DeviceId => await _logicMonitorClient
				.WriteLogAsync(
					logEventBatch.Select(logEvent => new WriteLogRequest(
							GetWriteLogLevel(logEvent),
							_deviceId ?? throw new InvalidOperationException($"{nameof(_deviceId)} should not be null for write method {_writeMethod}"),
							logEvent.RenderMessage(_formatProvider)
						)
					),
					cancellationToken: default
			).ConfigureAwait(false),
			WriteMethod.DeviceDisplayName => await _logicMonitorClient
				.WriteLogAsync(
					logEventBatch.Select(logEvent => new WriteLogRequest(
					GetWriteLogLevel(logEvent),
					_deviceDisplayName ?? throw new InvalidOperationException($"{nameof(_deviceDisplayName)} should not be null for write method {_writeMethod}"),
					logEvent.RenderMessage(_formatProvider)
				)
			),
			cancellationToken: default
			).ConfigureAwait(false),
			WriteMethod.CustomProperty => await _logicMonitorClient
				.WriteLogAsync(
					logEventBatch.Select(logEvent => new WriteLogRequest(
					GetWriteLogLevel(logEvent),
					_customPropertyName ?? throw new InvalidOperationException($"{nameof(_customPropertyName)} should not be null for write method {_writeMethod}"),
					_customPropertyValue ?? throw new InvalidOperationException($"{nameof(_customPropertyValue)} should not be null for write method {_writeMethod}"),
					logEvent.RenderMessage(_formatProvider)
				)),
				cancellationToken: default
			).ConfigureAwait(false),
			WriteMethod.PropertyDictionary => await _logicMonitorClient
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

	/// <summary>
	/// Called when a batching cycle has no events.
	/// </summary>
	/// <returns>A completed task.</returns>
	public Task OnEmptyBatchAsync() => Task.CompletedTask;

	private enum WriteMethod
	{
		DeviceId,
		DeviceDisplayName,
		CustomProperty,
		PropertyDictionary
	}

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
				_logicMonitorClient?.Dispose();
			}

			disposedValue = true;
		}
	}

	/// <summary>
	/// Releases sink resources.
	/// </summary>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}