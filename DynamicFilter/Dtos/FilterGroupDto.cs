// <copyright file="FilterGroupDto.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

namespace DynamicFilter.Dtos;

/// <summary>
/// FilterGroupDto
/// </summary>
[FilterGroupValidator]
public class FilterGroupDto : FilterItemDto
{
    /// <summary>
    /// Gets or sets Condition
    /// Values: "And", "Or"
    /// </summary>
    public string? Condition { get; set; }

    /// <summary>
    /// Gets or sets Filters
    /// Required, if Condition is set
    /// </summary>
    public List<FilterGroupDto>? Filters { get; set; }

    /// <summary>
    /// Gets or sets AnyFilter
    /// Filter that can be applied to Any method
    /// Required, only if Operator is "Any"
    /// </summary>
    public FilterGroupDto? AnyFilter { get; set; }
}
