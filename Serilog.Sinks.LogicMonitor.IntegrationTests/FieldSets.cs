namespace Serilog.Sinks.LogicMonitor.IntegrationTests;

/// <summary>
/// The field writer sets exercised by the integration tests.
/// </summary>
internal static class FieldSets
{
	private static readonly Dictionary<string, string> _pascalCaseNames = new()
	{
		["message"] = "Message",
		["message_template"] = "MessageTemplate",
		["level"] = "Level",
		["raise_date"] = "RaiseDate",
		["exception"] = "\"Exception\"",
		["properties"] = "Properties",
		["props_test"] = "PropsTest",
		["int_prop_test"] = "IntPropTest",
		["machine_name"] = "MachineName"
	};

	public static Dictionary<string, FieldWriterBase> Default() => Build(name => name, true);

	public static Dictionary<string, FieldWriterBase> WithQuotedNames()
		=> Build(name => name is "message_template" or "level" ? $"\"{name}\"" : name, false);

	public static Dictionary<string, FieldWriterBase> WithoutMachineName() => Build(name => name, false);

	public static Dictionary<string, FieldWriterBase> WithPascalCaseNames() => Build(name => _pascalCaseNames[name], true);

	private static Dictionary<string, FieldWriterBase> Build(Func<string, string> name, bool includeMachineName)
	{
		var fields = new Dictionary<string, FieldWriterBase>
		{
			[name("message")] = new RenderedMessageFieldWriter(),
			[name("message_template")] = new MessageTemplateFieldWriter(),
			[name("level")] = new LevelFieldWriter(true),
			[name("raise_date")] = new TimestampFieldWriter(),
			[name("exception")] = new ExceptionFieldWriter(),
			[name("properties")] = new LogEventSerializedFieldWriter(),
			[name("props_test")] = new PropertiesFieldWriter()
		};
		if (includeMachineName)
		{
			fields[name("int_prop_test")] = new SinglePropertyFieldWriter("testNo", PropertyWriteMethod.Raw);
			fields[name("machine_name")] = new SinglePropertyFieldWriter("MachineName", format: "l");
		}

		return fields;
	}
}
