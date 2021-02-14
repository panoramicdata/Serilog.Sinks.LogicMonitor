using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Linq;
using Xunit;

namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests
{
	public class LevelFieldWriterTest
	{
		[Fact]
		public void ByDefault_ShouldWriteLevelNo()
		{
			var writer = new LevelFieldWriter();
			var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
			var result = writer.GetValue(testEvent);

			result.Should().Be(1);
		}

		[Fact]
		public void WriteAsTextIsTrue_ShouldWriteLevelName()
		{
			var writer = new LevelFieldWriter(true);
			var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
			var result = writer.GetValue(testEvent);

			result.Should().Be(nameof(LogEventLevel.Debug));
		}
	}
}