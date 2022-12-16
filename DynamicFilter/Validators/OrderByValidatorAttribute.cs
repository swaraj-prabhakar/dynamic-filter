// <copyright file="OrderByValidatorAttribute.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

namespace DynamicFilter.Validators;

/// <summary>
/// OrderByValidatorAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class OrderByValidatorAttribute : ValidationAttribute
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
            if (value is OrderByDto filterDto)
            {
                if (string.IsNullOrEmpty(filterDto.Property) || string.IsNullOrWhiteSpace(filterDto.Property))
                {
                    ErrorMessage = "Property cannot be null or empty";
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
}
