using LogicMonitor.Api.Logging;

namespace Serilog.Sinks.LogicMonitor;

internal interface IWriteLogClient : IDisposable
{
	Task WriteLogAsync(IEnumerable<WriteLogRequest> requests, CancellationToken cancellationToken);
}
