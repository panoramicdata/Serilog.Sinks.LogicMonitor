using LogicMonitor.Api;
using LogicMonitor.Api.Logging;
using Serilog.Parsing;

namespace Serilog.Sinks.LogicMonitor.Tests;

/// <summary>
/// Tests for <see cref="LogicMonitorSink"/> using a recording client, so nothing leaves the process.
/// </summary>
public class LogicMonitorSinkTests
{
	private static readonly LogicMonitorClientOptions _options = new()
	{
		Account = "example",
		AccessId = "id",
		AccessKey = "key"
	};

	private static LogEvent CreateEvent(LogEventLevel level, string text = "hello")
		=> new(DateTimeOffset.UtcNow, level, null, new MessageTemplateParser().Parse(text), []);

	private static async Task<List<WriteLogRequest>> EmitAsync(LogicMonitorSink sink, params LogEvent[] events)
	{
		using (sink)
		{
			var client = new RecordingWriteLogClient();
			sink.ReplaceClient(client);
			await sink.EmitBatchAsync(events);
			return client.Requests;
		}
	}

	/// <summary>
	/// EmitBatchAsync MapsTheSerilogLevelToTheLogicMonitorLevel.
	/// </summary>
	[Theory]
	[InlineData(LogEventLevel.Fatal, "EROR")]
	[InlineData(LogEventLevel.Error, "EROR")]
	[InlineData(LogEventLevel.Warning, "WARN")]
	[InlineData(LogEventLevel.Information, "INFO")]
	[InlineData(LogEventLevel.Debug, "DEBG")]
	[InlineData(LogEventLevel.Verbose, "DEBG")]
	public async Task EmitBatchAsync_MapsTheSerilogLevelToTheLogicMonitorLevel(LogEventLevel level, string expectedTag)
	{
		var requests = await EmitAsync(new LogicMonitorSink(_options, 7), CreateEvent(level));

		requests.Should().ContainSingle();
		((string)requests[0]["message"]).Should().Be($"[{expectedTag}] hello");
	}

	/// <summary>
	/// EmitBatchAsync WithAResourceId IdentifiesTheDeviceById.
	/// </summary>
	[Fact]
	public async Task EmitBatchAsync_WithAResourceId_IdentifiesTheDeviceById()
	{
		var requests = await EmitAsync(new LogicMonitorSink(_options, 7), CreateEvent(LogEventLevel.Information));

		var resource = (Dictionary<string, string>)requests[0]["_lm.resourceId"];
		resource.Values.Should().Contain("7");
	}

	/// <summary>
	/// EmitBatchAsync WithADisplayName IdentifiesTheDeviceByName.
	/// </summary>
	[Fact]
	public async Task EmitBatchAsync_WithADisplayName_IdentifiesTheDeviceByName()
	{
		var requests = await EmitAsync(new LogicMonitorSink(_options, "the-device"), CreateEvent(LogEventLevel.Information));

		var resource = (Dictionary<string, string>)requests[0]["_lm.resourceId"];
		resource.Values.Should().Contain("the-device");
	}

	/// <summary>
	/// EmitBatchAsync WithACustomProperty IdentifiesTheDeviceByThatProperty.
	/// </summary>
	[Fact]
	public async Task EmitBatchAsync_WithACustomProperty_IdentifiesTheDeviceByThatProperty()
	{
		var requests = await EmitAsync(new LogicMonitorSink(_options, "cmdb.id", "abc"), CreateEvent(LogEventLevel.Information));

		var resource = (Dictionary<string, string>)requests[0]["_lm.resourceId"];
		resource.Should().Contain("cmdb.id", "abc");
	}

	/// <summary>
	/// EmitBatchAsync WithAPropertyDictionary IdentifiesTheDeviceByAllTheProperties.
	/// </summary>
	[Fact]
	public async Task EmitBatchAsync_WithAPropertyDictionary_IdentifiesTheDeviceByAllTheProperties()
	{
		var properties = new Dictionary<string, string> { ["a"] = "1", ["b"] = "2" };

		var requests = await EmitAsync(new LogicMonitorSink(_options, properties), CreateEvent(LogEventLevel.Information));

		var resource = (Dictionary<string, string>)requests[0]["_lm.resourceId"];
		resource.Should().BeEquivalentTo(properties);
	}

	/// <summary>
	/// EmitBatchAsync WithAFormatProvider RendersTheMessageWithIt.
	/// </summary>
	[Fact]
	public async Task EmitBatchAsync_WithAFormatProvider_RendersTheMessageWithIt()
	{
		var template = new MessageTemplateParser().Parse("value {Number}");
		var logEvent = new LogEvent(
			DateTimeOffset.UtcNow,
			LogEventLevel.Information,
			null,
			template,
			[new LogEventProperty("Number", new ScalarValue(1.5))]);

		var requests = await EmitAsync(
			new LogicMonitorSink(_options, 7, System.Globalization.CultureInfo.GetCultureInfo("de-DE")),
			logEvent);

		((string)requests[0]["message"]).Should().Be("[INFO] value 1,5");
	}

	/// <summary>
	/// EmitBatchAsync WithSeveralEvents WritesOneRequestPerEvent.
	/// </summary>
	[Fact]
	public async Task EmitBatchAsync_WithSeveralEvents_WritesOneRequestPerEvent()
	{
		var requests = await EmitAsync(
			new LogicMonitorSink(_options, 7),
			CreateEvent(LogEventLevel.Information, "one"),
			CreateEvent(LogEventLevel.Error, "two"));

		requests.Should().HaveCount(2);
	}

	/// <summary>
	/// Constructor WithAnInvalidResourceId Throws.
	/// </summary>
	[Fact]
	public void Constructor_WithAnInvalidResourceId_Throws()
	{
		var act = () => new LogicMonitorSink(_options, 0);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Constructor WithNoOptions Throws.
	/// </summary>
	[Fact]
	public void Constructor_WithNoOptions_Throws()
	{
		var act = () => new LogicMonitorSink(null!, 7);

		act.Should().Throw<ArgumentNullException>();
	}

	/// <summary>
	/// Constructor WithANullPropertyDictionary Throws.
	/// </summary>
	[Fact]
	public void Constructor_WithANullPropertyDictionary_Throws()
	{
		var act = () => new LogicMonitorSink(_options, (Dictionary<string, string>)null!);

		act.Should().Throw<ArgumentNullException>();
	}

	/// <summary>
	/// Constructor WithoutFieldOptions UsesTheDefaults.
	/// </summary>
	[Fact]
	public void Constructor_WithoutFieldOptions_UsesTheDefaults()
	{
		using var sink = new LogicMonitorSink(_options, 7);

		sink.FieldOptionsInUse.Should().ContainKey(DefaultFieldNames.Level);
	}

	/// <summary>
	/// OnEmptyBatchAsync Completes.
	/// </summary>
	[Fact]
	public async Task OnEmptyBatchAsync_Completes()
	{
		using var sink = new LogicMonitorSink(_options, 7);

		await sink.OnEmptyBatchAsync();
	}

	/// <summary>
	/// Dispose DisposesTheClientOnce.
	/// </summary>
	[Fact]
	public void Dispose_DisposesTheClientOnce()
	{
		var sink = new LogicMonitorSink(_options, 7);
		var client = new RecordingWriteLogClient();
		sink.ReplaceClient(client);

		sink.Dispose();
		sink.Dispose();

		client.Disposed.Should().BeTrue();
	}
}
