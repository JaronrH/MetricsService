using FluentAssertions;
using MetricsService.Extensions;

namespace MetricsService.Tests;

public class StringExtensionTests
{
    /// <summary>
    /// ToDotNotation() Test Cases
    /// </summary>
    public static object[][] DotNotationTestCases = [
        ["This is a String.", "this.is.a.string", false],
        ["This is a StringThing.", "this.is.a.stringThing", false],
        ["StringThing", "stringThing", false],
        ["   ", "", false],
        ["this.Is.A string", "this.Is.A.string", false],
        ["this.Is.A string", "this.is.a.string", true]
    ];

    /// <summary>
    /// Validate Input/Output of ToDotNotation()
    /// </summary>
    /// <param name="input">Input</param>
    /// <param name="expectedOutput">Expected Output</param>
    /// <param name="ensureCamelCases">Make sure final result follows Dot Notation.  For example: "this.Is.A string" -> "this.Is.A.string" when false but "this.is.a.string" when true.</param>
    [Theory]
    [MemberData("DotNotationTestCases")]
    public void DotNotationTests(string input, string expectedOutput, bool ensureCamelCases) =>
        input.ToDotNotation(ensureCamelCases).Should().Be(expectedOutput);
}