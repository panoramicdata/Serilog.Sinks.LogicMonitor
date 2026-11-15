using LogicMonitor.Api;
using LogicMonitor.Api.Logging;

namespace Serilog.Sinks.LogicMonitor;

internal sealed class LogicMonitorWriteLogClient(LogicMonitorClientOptions options) : IWriteLogClient
{
	private readonly LogicMonitorClient _client = new(options);

	public Task WriteLogAsync(IEnumerable<WriteLogRequest> requests, CancellationToken cancellationToken)
		=> _client.WriteLogAsync(requests, cancellationToken);

	public void Dispose() => _client.Dispose();
}
