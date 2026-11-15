using LogicMonitor.Api.Logging;

namespace Serilog.Sinks.LogicMonitor.Tests;

internal sealed class RecordingWriteLogClient : IWriteLogClient
{
	public List<WriteLogRequest> Requests { get; } = [];

	public bool Disposed { get; private set; }

	public Task WriteLogAsync(IEnumerable<WriteLogRequest> requests, CancellationToken cancellationToken)
	{
		Requests.AddRange(requests);
		return Task.CompletedTask;
	}

	public void Dispose() => Disposed = true;
}
