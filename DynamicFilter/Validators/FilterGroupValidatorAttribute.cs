// <copyright file="FilterGroupValidatorAttribute.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

namespace DynamicFilter.Validators;

/// <summary>
/// FilterGroupValidatorAttribute
/// </summary>
internal sealed class FilterGroupValidatorAttribute : ValidationAttribute
{
    /// <summary>
    /// IsValid
    /// </summary>
    /// <param name="value">value</param>
    /// <returns>bool</returns>
    public override bool IsValid(object? value)
    {
        if (value != null)
        {
            if (value is FilterGroupDto filterGroupDto)
            {
                if (IsValidString(filterGroupDto.Condition))
                {
                    if (!IsValidEnumName<Condition>(filterGroupDto.Condition))
                    {
                        ErrorMessage = "Invalid condition";
                        return false;
                    }
                    if (filterGroupDto.Filters == null || !filterGroupDto.Filters.Any())
                    {
                        ErrorMessage = $"Filters must be non-empty, if condition is set and valid";
                        return false;
                    }
                }
                if (IsValidString(filterGroupDto.Operator))
                {
                    if (!IsValidEnumName<Operator>(filterGroupDto.Operator))
                    {
                        ErrorMessage = "Invalid operator";
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private static bool IsValidEnumName<T>(string? enumMember)
    {
        return Enum.GetNames(typeof(T)).Contains(enumMember);
    }
    private static bool IsValidString(string? value)
    {
        return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
    }
}
