using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Linq;
using Xunit;

namespace Serilog.Sinks.LogicMonitor.Tests.FieldWritersTests
{
	public class PropertiesFieldWriterTest
	{
		[Fact]
		public void NoProperties_ShouldReturnEmptyJsonObject()
		{
			var writer = new PropertiesFieldWriter();
			var testEvent = new LogEvent(DateTime.Now, LogEventLevel.Debug, null, new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
			var result = writer.GetValue(testEvent);

			result.Should().Be("{}");
		}
	}
}