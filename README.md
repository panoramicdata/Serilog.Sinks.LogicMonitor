# Serilog.Sinks.LogicMonitor
A [Serilog](https://github.com/serilog/serilog) sink that writes to LogicMonitor.

NOTE: **FieldWriters are not yet used and can be omitted.**

**Package** - [Serilog.Sinks.LogicMonitor](https://www.nuget.org/packages/Serilog.Sinks.LogicMonitor.PanoramicData/)
| **Platforms** - .NET Standard 2.0

#### Code

```csharp
var logicMonitorClientOptions = new LogicMonitorClientOptions
	{
		Account = "acme",
		AccessId = "The access token's id",
		AccessKey = "The access token's key",
	};

// Used fields (Key is a field name) 
// Field type is writer's constructor parameter
var fieldWriters = new Dictionary<string, FieldWriterBase>
	{
		 {"message", new RenderedMessageFieldWriter() },
		 {"message_template", new MessageTemplateFieldWriter() },
		 {"level", new LevelFieldWriter(true) },
		 {"raise_date", new TimestampFieldWriter() },
		 {"exception", new ExceptionFieldWriter() },
		 {"properties", new LogEventSerializedFieldWriter() },
		 {"props_test", new PropertiesFieldWriter() },
		 {"machine_name", new SinglePropertyFieldWriter("MachineName", PropertyWriteMethod.ToString, "l") }
	};

var logger = new LoggerConfiguration()
	.WriteTo.LogicMonitor(logicMonitorClientOptions, fieldWriters)
	.CreateLogger();
```