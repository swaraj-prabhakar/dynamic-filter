// <copyright file="FilterItemDto.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

using Newtonsoft.Json;

namespace DynamicFilter.Dtos;

/// <summary>
/// FilterItemDto
/// </summary>
public class FilterItemDto
{
    /// <summary>
    /// Gets or sets Property
    /// Property Name or Column Name
    /// Ignored, if condition is set
    /// </summary>
    public string? Property { get; set; }

    /// <summary>
    /// Gets or sets Operator
    /// Operator that can be applied to Property
    /// Values: Equal, NotEqual, GreaterThan, LessThan, GreaterThanOrEqual,
    /// LessThanOrEqual, Between, BetweenInclusive, In, NotIn, Contains, NotContains, Any
    /// </summary>
    public string? Operator { get; set; }

    /// <summary>
    /// Gets or sets Value
    /// Examples: 1, "Hello", "H", 100, 222.01, "2022-12-15 10:10:00", true,
    /// [1,2,3], ["Hello", "List", "Values"], [22.43, 45.232, 454.56], ["2022-12-15 10:10:00", 2022-12-25 10:10:00]
    /// </summary>
    [JsonConverter(typeof(ValueConverter))]
    public dynamic? Value { get; set; }

    /// <summary>
    /// Gets or sets CaseSensitive
    /// Default value is false
    /// Determines whether the filter should be applied without considering case
    /// </summary>
    public bool CaseSensitive { get; set; } = false;
}
