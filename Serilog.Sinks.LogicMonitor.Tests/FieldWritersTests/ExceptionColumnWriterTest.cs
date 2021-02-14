using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Linq;
using Xunit;

namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests
{
	public class ExceptionColumnWriterTest
	{
		[Fact]
		public void ExceptionIsNull_ShouldReturnNullValue()
		{
			var writer = new ExceptionFieldWriter();
			var testEvent = new LogEvent(
				DateTime.Now,
				LogEventLevel.Debug,
				null, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
			var result = writer.GetValue(testEvent);

			result.Should().BeNull();
		}

		[Fact]
		public void ExceptionIsPresent_ShouldReturnStringrepresentation()
		{
			var writer = new ExceptionFieldWriter();
			var exception = new Exception("Test exception");
			var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, exception, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
			var result = writer.GetValue(testEvent);

			result.Should().Be(exception.ToString());
		}
	}
}