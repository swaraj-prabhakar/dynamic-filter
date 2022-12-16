// <copyright file="OrderByDto.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

namespace DynamicFilter.Dtos;

/// <summary>
/// OrderByDto
/// </summary>
[OrderByValidator]
public class OrderByDto
{
    /// <summary>
    /// Gets or sets Property
    /// Property Name or Column Name
    /// </summary>
    public string Property { get; set; }

    /// <summary>
    /// Gets or sets Ascending
    /// Whether the list can be sorted in ascending or descending order
    /// Default Value = true
    /// </summary>
    public bool Ascending { get; set; } = true;
}
