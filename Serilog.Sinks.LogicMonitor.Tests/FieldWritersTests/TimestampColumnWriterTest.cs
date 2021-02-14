using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Linq;
using Xunit;

namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests
{
	public class TimestampColumnWriterTest
	{
		[Fact]
		public void ByDefault_ShouldReturnTimestampValueWithoutTimezone()
		{
			var writer = new TimestampFieldWriter();
			var timeStamp = new DateTimeOffset(2017, 8, 13, 11, 11, 11, new TimeSpan());
			var testEvent = new LogEvent(timeStamp, LogEventLevel.Debug, null, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
			var result = writer.GetValue(testEvent);

			result.Should().Be(timeStamp);
		}

		[Fact]
		public void DbTypeWithTimezoneSelected_ShouldReturnTimestampValue()
		{
			var writer = new TimestampFieldWriter();
			var timeStamp = new DateTimeOffset(2017, 8, 13, 11, 11, 11, new TimeSpan());
			var testEvent = new LogEvent(timeStamp, LogEventLevel.Debug, null, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());

			var result = writer.GetValue(testEvent);

			result.Should().Be(timeStamp);
		}
	}
}
