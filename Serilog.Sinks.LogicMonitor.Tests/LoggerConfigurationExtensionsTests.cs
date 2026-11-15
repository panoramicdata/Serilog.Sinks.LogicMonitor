using LogicMonitor.Api;
using Serilog.Configuration;

namespace Serilog.Sinks.LogicMonitor.Tests;

/// <summary>
/// Tests for <see cref="LoggerConfigurationLogicMonitorExtensions"/>.
/// </summary>
public class LoggerConfigurationExtensionsTests
{
	private static readonly LogicMonitorClientOptions _options = new()
	{
		Account = "example",
		AccessId = "id",
		AccessKey = "key"
	};

	private static LoggerSinkConfiguration Sink => new LoggerConfiguration().WriteTo;

	/// <summary>
	/// Argument sets that build a logger.
	/// </summary>
	public static TheoryData<string, Func<LoggerSinkConfiguration, LoggerConfiguration>> ValidConfigurations => new()
	{
		{ "resource id", sink => sink.LogicMonitor(_options, 7) },
		{ "display name", sink => sink.LogicMonitor(_options, "device") },
		{ "custom property", sink => sink.LogicMonitor(_options, "cmdb.id", "abc") },
		{ "property dictionary", sink => sink.LogicMonitor(_options, new Dictionary<string, string> { ["a"] = "b" }) }
	};

	/// <summary>
	/// Argument sets that are rejected.
	/// </summary>
	public static TheoryData<string, Action> InvalidArguments => new()
	{
		{ "null configuration", () => LoggerConfigurationLogicMonitorExtensions.LogicMonitor(null!, _options, 7) },
		{ "null options", () => Sink.LogicMonitor(null!, 7) },
		{ "zero resource id", () => Sink.LogicMonitor(_options, 0) },
		{ "null display name", () => Sink.LogicMonitor(_options, (string)null!) },
		{ "null property name", () => Sink.LogicMonitor(_options, (string)null!, "value") },
		{ "null property value", () => Sink.LogicMonitor(_options, "name", (string)null!) },
		{ "null dictionary", () => Sink.LogicMonitor(_options, (Dictionary<string, string>)null!) }
	};

	/// <summary>
	/// LogicMonitor WithValidArguments CreatesALogger.
	/// </summary>
	[Theory]
	[MemberData(nameof(ValidConfigurations))]
	public void LogicMonitor_WithValidArguments_CreatesALogger(string description, Func<LoggerSinkConfiguration, LoggerConfiguration> configure)
	{
		Assert.NotEmpty(description);

		using var logger = configure(Sink).CreateLogger();

		logger.Should().NotBeNull();
	}

	/// <summary>
	/// LogicMonitor WithInvalidArguments Throws.
	/// </summary>
	[Theory]
	[MemberData(nameof(InvalidArguments))]
	public void LogicMonitor_WithInvalidArguments_Throws(string description, Action act)
	{
		Assert.NotEmpty(description);

		act.Should().Throw<ArgumentException>();
	}

	/// <summary>
	/// LogicMonitor WithADisplayNameContainingTheHostnameToken ReplacesIt.
	/// </summary>
	[Fact]
	public void LogicMonitor_WithADisplayNameContainingTheHostnameToken_ReplacesIt()
	{
		using var logger = Sink.LogicMonitor(_options, "{EnvironmentVariable:HOSTNAME}-device").CreateLogger();

		logger.Should().NotBeNull();
	}

	/// <summary>
	/// DefaultPeriod IsFiveSeconds.
	/// </summary>
	[Fact]
	public void DefaultPeriod_IsFiveSeconds()
		=> LoggerConfigurationLogicMonitorExtensions.DefaultPeriod.Should().Be(TimeSpan.FromSeconds(5));
}
