namespace Serilog.Sinks.LogicMonitor.IntegrationTests.Objects;

/// <summary>
/// A simple test object type used for testing the PropertiesFieldWriter, which writes all properties of a log event as individual fields in LogicMonitor. This class has two properties, StringProp and IntProp, which can be used to verify that the PropertiesFieldWriter correctly extracts and writes the properties from a log event when it is included in the field properties for the LogicMonitor sink. By using this test object type in log events during the integration tests, we can confirm that the PropertiesFieldWriter is functioning as expected and that the properties are being written to LogicMonitor correctly.
/// </summary>
public class TestObjectType1
{
	/// <summary>
	/// A string property used for testing the PropertiesFieldWriter. This property can be set to any string value and will be included as a field in LogicMonitor when a log event containing an instance of TestObjectType1 is written with the PropertiesFieldWriter configured in the field properties for the LogicMonitor sink. By verifying that this property is correctly extracted and written to LogicMonitor, we can confirm that the PropertiesFieldWriter is functioning as expected for string properties on log event objects.
	/// </summary>
	public required string StringProp { get; set; }

	/// <summary>
	/// An integer property used for testing the PropertiesFieldWriter. This property can be set to any integer value and will be included as a field in LogicMonitor when a log event containing an instance of TestObjectType1 is written with the PropertiesFieldWriter configured in the field properties for the LogicMonitor sink. By verifying that this property is correctly extracted and written to LogicMonitor, we can confirm that the PropertiesFieldWriter is functioning as expected for integer properties on log event objects.
	/// </summary>
	public required int IntProp { get; set; }
}