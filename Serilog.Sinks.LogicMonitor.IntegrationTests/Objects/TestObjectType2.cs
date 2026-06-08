namespace Serilog.Sinks.LogicMonitor.IntegrationTests.Objects;

/// <summary>
/// A test object type used for testing nested properties with the PropertiesFieldWriter. This class has a DateTime property (DateProp1) and a nested property of type TestObjectType1 (NestedProp). By including an instance of TestObjectType2 in log events during the integration tests, we can verify that the PropertiesFieldWriter correctly extracts and writes both the DateTime property and the nested properties from the TestObjectType1 instance to LogicMonitor as individual fields. This allows us to confirm that the PropertiesFieldWriter is functioning as expected for more complex objects with nested properties when writing log events to LogicMonitor.
/// </summary>
public class TestObjectType2
{
	/// <summary>
	/// A DateTime property used for testing the PropertiesFieldWriter. This property can be set to any DateTime value and will be included as a field in LogicMonitor when a log event containing an instance of TestObjectType2 is written with the PropertiesFieldWriter configured in the field properties for the LogicMonitor sink. By verifying that this property is correctly extracted and written to LogicMonitor, we can confirm that the PropertiesFieldWriter is functioning as expected for DateTime properties on log event objects.
	/// </summary>
	public required DateTime DateProp1 { get; set; }

	/// <summary>
	/// A nested property of type TestObjectType1 used for testing the PropertiesFieldWriter. This property can be set to an instance of TestObjectType1 with its own properties (StringProp and IntProp), and when a log event containing an instance of TestObjectType2 is written with the PropertiesFieldWriter configured in the field properties for the LogicMonitor sink, the PropertiesFieldWriter should correctly extract and write the properties from the nested TestObjectType1 instance to LogicMonitor as individual fields. By verifying that both the DateTime property and the nested properties from TestObjectType1 are correctly extracted and written to LogicMonitor, we can confirm that the PropertiesFieldWriter is functioning as expected for complex objects with nested properties when writing log events to LogicMonitor.
	/// </summary>
	public required TestObjectType1 NestedProp { get; set; }
}