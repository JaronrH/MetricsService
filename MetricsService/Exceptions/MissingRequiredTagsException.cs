using System;
using System.Collections.Generic;

namespace MetricsService.Exceptions;

/// <summary>
/// Missing Required Tag Names.
/// </summary>
/// <param name="missingTagNames">Missing Tag Names</param>
public class MissingRequiredTagsException(IEnumerable<string> missingTagNames) : ArgumentException($"Missing Required Tag(s): {string.Join(", ", missingTagNames)}")
{
    /// <summary>
    /// Missing Tag Names
    /// </summary>
    public IEnumerable<string> MissingTagNames { get; set; } = missingTagNames;
}