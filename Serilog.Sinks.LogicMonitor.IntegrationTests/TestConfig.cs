using LogicMonitor.Api;

namespace Serilog.Sinks.LogicMonitor.IntegrationTests
{
	public class TestConfig
	{
		public LogicMonitorClientOptions ClientOptions { get; set; }

		public int DeviceId { get; set; }
	}
}