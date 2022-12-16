// <copyright file="FilterDto.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

using Newtonsoft.Json;

namespace DynamicFilter.Dtos;

/// <summary>
/// FilterDto
/// </summary>
public class FilterDto
{
    /// <summary>
    /// Gets or sets Filter
    /// Can be applied to Where clause
    /// </summary>
    public FilterGroupDto? Filter { get; set; }

    /// <summary>
    /// Gets or sets OrderBy
    /// Can be applied to OrderBy or OrderByDescending clause
    /// </summary>
    public List<OrderByDto>? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets Skip
    /// Used for Linq Skip method
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// Gets or sets Take
    /// Used for Linq Take method
    /// </summary>
    public int? Take { get; set; }
}
