// <copyright file="Operator.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

namespace DynamicFilter.Enums;

/// <summary>
/// Operator
/// </summary>
public enum Operator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Between,
    BetweenInclusive,
    In,
    NotIn,
    Contains,
    NotContains,
    Any,
}
