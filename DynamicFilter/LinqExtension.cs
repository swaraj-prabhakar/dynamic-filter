// <copyright file="LinqExtension.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

namespace DynamicFilter;

/// <summary>
/// An extension class containing methods to perform dynamic filtering 
/// </summary>
public static class LinqExtension
{
    #region properties

    /// <summary>
    /// String.Contains method
    /// </summary>
    private static readonly MethodInfo stringContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

    /// <summary>
    /// Enumerable.Contains method
    /// </summary>
    private static readonly MethodInfo listContainsMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

    /// <summary>
    /// String.ToLower method
    /// </summary>
    private static readonly MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

    /// <summary>
    /// Enumerable.Any(predicate) method
    /// </summary>
    private static readonly MethodInfo anyConditionMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2);

    /// <summary>
    /// Enumerable.Any method
    /// </summary>
    private static readonly MethodInfo anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 1);

    /// <summary>
    /// BuildPredicateMethod of the same class
    /// </summary>
    private static readonly MethodInfo buildPredicateMethod = typeof(LinqExtension).GetMethod("BuildPredicateAny", BindingFlags.NonPublic | BindingFlags.Static);

    /// <summary>
    /// Delegate for binding two expressions (Expression.And or Expression.Or)
    /// </summary>
    private delegate Expression AndOr(Expression lhs, Expression rhs);
    #endregion properties

    #region wherePredicate

    /// <summary>
    /// Parse filter conditions
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="filter">filter</param>
    /// <param name="param">param</param>
    /// <returns>Expression that can be converted to lambda expression</returns>
    private static Expression ParseFilterConditions<T>(JsonElement filter, ParameterExpression param)
    {
        Expression lhs = null;
        var conditionString = filter.GetProperty("Condition").GetString();
        Condition? condition = default;
        if (conditionString != null)
        {
            condition = Enum.Parse<Condition>(filter.GetProperty("Condition").GetString());
        }

        bool caseSensitive = filter.GetProperty("CaseSensitive").GetBoolean();

        AndOr binder = condition.HasValue && condition.Value == Condition.And ? Expression.And : Expression.Or;
        Expression bind(Expression left, Expression right) => left == null ? right : binder(left, right);

        if (filter.TryGetProperty("Filters", out JsonElement checkFilters) && checkFilters.ValueKind != JsonValueKind.Null)
        {
            foreach (var filterItem in filter.GetProperty("Filters").EnumerateArray())
            {
                if (condition != null)
                {
                    var rhs = ParseFilterConditions<T>(filterItem, param);
                    lhs = bind(lhs, rhs);
                }
            }
        }

        if (conditionString == null)
        {
            Operator? op = Enum.Parse<Operator>(filter.GetProperty("Operator").GetString());
            PropertyType? propertyType = null;
            string? propertyName = filter.GetProperty("Property").GetString();
            dynamic? value = default;
            JsonElement valueElement = filter.GetProperty("Value");

            var memberExpr = GetMemberExpression(param, propertyName);
            Type memberType = ((memberExpr as MemberExpression).Member as PropertyInfo).PropertyType;

            propertyType = memberType switch
            {
                var x when (x == typeof(string)) => PropertyType.String,
                var x when (x == typeof(short) || x == typeof(short?)) => PropertyType.Short,
                var x when (x == typeof(int) || x == typeof(int?)) => PropertyType.Int,
                var x when (x == typeof(long) || x == typeof(long?)) => PropertyType.Long,
                var x when (x == typeof(double) || x == typeof(double?)) => PropertyType.Double,
                var x when (x == typeof(decimal) || x == typeof(decimal?)) => PropertyType.Decimal,
                var x when (x == typeof(DateTime) || x == typeof(DateTime?)) => PropertyType.DateTime,
                var x when (x == typeof(bool) || x == typeof(bool?)) => PropertyType.Boolean,
                _ => PropertyType.String,
            };

            if(op != null && propertyType != null && op != Operator.Any && !IsOperatorMatchingType((Operator)op, (PropertyType)propertyType))
            {
                throw new Exception($"The operator \"{op}\" is not applicable on the property \"{propertyName}\" of type \"{propertyType}\".");
            }

            bool caseApplicable = !caseSensitive && op != null && propertyType.HasValue && IsCaseApplicable(op.Value, propertyType.Value);

            var propertyExp = caseApplicable ? Expression.Call(memberExpr, toLowerMethod) : memberExpr;
            ConstantExpression constExp;

            if (op == Operator.In || op == Operator.NotIn || op == Operator.Between)
            {
                value = propertyType switch
                {
                    PropertyType.Short => valueElement.EnumerateArray().Select(s => s.GetInt16()).ToList(),
                    PropertyType.Int => valueElement.EnumerateArray().Select(s => s.GetInt32()).ToList(),
                    PropertyType.Long => valueElement.EnumerateArray().Select(s => s.GetInt64()).ToList(),
                    PropertyType.Double => valueElement.EnumerateArray().Select(s => s.GetDouble()).ToList(),
                    PropertyType.Decimal => valueElement.EnumerateArray().Select(s => s.GetDecimal()).ToList(),
                    PropertyType.String => caseApplicable ? valueElement.EnumerateArray().Select(s => s.GetString().ToLower()).ToList() : valueElement.EnumerateArray().Select(s => s.GetString()).ToList(),
                    PropertyType.DateTime => valueElement.EnumerateArray().Select(s => s.GetDateTime()).ToList(),
                    _ => valueElement.EnumerateArray().Select(s => s.GetString()).ToList(),
                };
            }
            else if (op != Operator.Any)
            {
                value = propertyType switch
                {
                    PropertyType.Short => valueElement.GetInt16(),
                    PropertyType.Int => valueElement.GetInt32(),
                    PropertyType.Long => valueElement.GetInt64(),
                    PropertyType.Double => valueElement.GetDouble(),
                    PropertyType.Decimal => valueElement.GetDecimal(),
                    PropertyType.String => caseApplicable ? valueElement.GetString().ToLower() : valueElement.GetString(),
                    PropertyType.DateTime => valueElement.GetDateTime(),
                    PropertyType.Boolean => valueElement.GetBoolean(),
                    _ => valueElement.GetString(),
                };
            }


            switch (op)
            {
                case Operator.Equal:
                    constExp = Expression.Constant(value);
                    lhs = Expression.Equal(propertyExp, constExp);
                    break;
                case Operator.NotEqual:
                    constExp = Expression.Constant(value);
                    lhs = Expression.NotEqual(propertyExp, constExp);
                    break;
                case Operator.GreaterThan:
                    constExp = Expression.Constant(value);
                    lhs = Expression.GreaterThan(propertyExp, constExp);
                    break;
                case Operator.LessThan:
                    constExp = Expression.Constant(value);
                    lhs = Expression.LessThan(propertyExp, constExp);
                    break;
                case Operator.GreaterThanOrEqual:
                    constExp = Expression.Constant(value);
                    lhs = Expression.GreaterThanOrEqual(propertyExp, constExp);
                    break;
                case Operator.LessThanOrEqual:
                    constExp = Expression.Constant(value);
                    lhs = Expression.LessThanOrEqual(propertyExp, constExp);
                    break;
                case Operator.Between:
                    constExp = Expression.Constant(value[0]);
                    var constExpB2 = Expression.Constant(value[1]);
                    var leftB = Expression.GreaterThan(propertyExp, constExp);
                    var rightB = Expression.LessThan(propertyExp, constExpB2);
                    lhs = Expression.AndAlso(leftB, rightB);
                    break;
                case Operator.BetweenInclusive:
                    constExp = Expression.Constant(value[0]);
                    var constExpBI2 = Expression.Constant(value[1]);
                    var leftBI = Expression.GreaterThanOrEqual(propertyExp, constExp);
                    var rightBI = Expression.LessThanOrEqual(propertyExp, constExpBI2);
                    lhs = Expression.AndAlso(leftBI, rightBI);
                    break;
                case Operator.In:
                    constExp = Expression.Constant(value);
                    var contains1 = listContainsMethod.MakeGenericMethod(value.GetType().GetGenericArguments()[0]);
                    lhs = Expression.Call(contains1, constExp, propertyExp);
                    break;
                case Operator.NotIn:
                    constExp = Expression.Constant(value);
                    var contains2 = listContainsMethod.MakeGenericMethod(value.GetType().GetGenericArguments()[0]);
                    lhs = Expression.Not(Expression.Call(contains2, constExp, propertyExp));
                    break;
                case Operator.Contains:
                    constExp = Expression.Constant(value);
                    lhs = Expression.Call(propertyExp, stringContainsMethod, constExp);
                    break;
                case Operator.NotContains:
                    constExp = Expression.Constant(value);
                    lhs = Expression.Not(Expression.Call(propertyExp, stringContainsMethod, constExp));
                    break;
                case Operator.Any:
                    dynamic typeOfArray = propertyExp.Type.GetGenericArguments()[0];
                    if (filter.TryGetProperty("AnyFilter", out JsonElement anyFilter) && anyFilter.ValueKind != JsonValueKind.Null && anyFilter.EnumerateObject().Any())
                    {
                        MethodInfo predicateMethod = buildPredicateMethod.MakeGenericMethod(typeOfArray);
                        var anyFilterExpr = (Expression)predicateMethod.Invoke(null, new[] { anyFilter.Deserialize<FilterGroupDto>() });
                        var methodAny = anyConditionMethod.MakeGenericMethod(typeOfArray);
                        lhs = Expression.Call(methodAny, propertyExp, anyFilterExpr);
                    }
                    else
                    {
                        var methodAny = anyMethod.MakeGenericMethod(typeOfArray);
                        lhs = Expression.Call(methodAny, propertyExp);
                    }
                    break;
            };
        }

        return lhs;
    }

    /// <summary>
    /// Get member expression
    /// </summary>
    /// <param name="param">param</param>
    /// <param name="property">property</param>
    /// <returns>Member expression</returns>
    private static Expression GetMemberExpression(Expression param, string property)
    {
        if (property.Contains('.'))
        {
            int index = property.IndexOf('.');
            var subParam = Expression.Property(param, property[..index]);
            return GetMemberExpression(subParam, property[(index + 1)..]);
        }

        return Expression.Property(param, property);
    }

    /// <summary>
    /// Build predicate for where condition with given filters
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="filter">filter</param>
    /// <returns>Predicate function that can be applied to where clause</returns>
    private static Func<T, bool> BuildPredicate<T>(FilterGroupDto filter)
    {
        var param = Expression.Parameter(typeof(T)); // (x =>)
        var jsonObj = JsonDocument.Parse(JsonSerializer.Serialize(filter));

        var conditions = ParseFilterConditions<T>(jsonObj.RootElement, param);
        if (conditions.CanReduce)
        {
            conditions = conditions.ReduceAndCheck();
        }
        var query = Expression.Lambda<Func<T, bool>>(conditions, param);
        return query.Compile();
    }

    /// <summary>
    /// Build predicate for Any() operator
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="filter">filter</param>
    /// <returns>Predicate expression that can be applied to Any() method</returns>
    private static Expression<Func<T, bool>> BuildPredicateAny<T>(FilterGroupDto filter)
    {
        var param = Expression.Parameter(typeof(T)); // (x =>)
        var jsonObj = JsonDocument.Parse(JsonSerializer.Serialize(filter));

        var conditions = ParseFilterConditions<T>(jsonObj.RootElement, param);
        if (conditions.CanReduce)
        {
            conditions = conditions.ReduceAndCheck();
        }

        return Expression.Lambda<Func<T, bool>>(conditions, param);
    }

    /// <summary>
    /// Check whether the case sensitivity can be applied to the given combination of operator and property type
    /// </summary>
    /// <param name="op">operator</param>
    /// <param name="type">property type</param>
    /// <returns>True, if case conversion can be applied; else, False</returns>
    private static bool IsCaseApplicable(Operator op, PropertyType type)
    {
        return type == PropertyType.String && new List<Operator>()
        {
            Operator.Equal,
            Operator.NotEqual,
            Operator.Contains,
            Operator.NotContains,
            Operator.In,
            Operator.NotIn
        }.Contains(op);
    }

    #endregion wherePredicate

    #region orderBy

    /// <summary>
    /// Get a function that can be applied to orderby clause
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="propertyName">propertyName</param>
    /// <returns>Predicate function that can be applied to orderby clause</returns>
    private static Func<T, object> GetOrderByFunc<T>(string propertyName)
    {
        var param = Expression.Parameter(typeof(T));
        Expression conversion = Expression.Convert(Expression.Property(param, propertyName), typeof(object));
        return Expression.Lambda<Func<T, object>>(conversion, param).Compile();
    }

    /// <summary>
    /// Apply order by condition to IAsyncEnumerable<Entity>
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="orders">orderby</param>
    /// <returns>IOrderedAsyncEnumerable</returns>
    private static IOrderedAsyncEnumerable<T> ApplyOrderBy<T>(this IAsyncEnumerable<T> enumerable, List<OrderByDto> orders)
    {
        if (orders[0].Ascending)
        {
            return enumerable.OrderBy(GetOrderByFunc<T>(orders[0].Property));
        }
        else
        {
            return enumerable.OrderByDescending(GetOrderByFunc<T>(orders[0].Property));
        }
    }

    /// <summary>
    /// Apply then by condition to IOrderedAsyncEnumerable<Entity>
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="orders">orderby</param>
    /// <returns>IOrderedAsyncEnumerable</returns>
    private static IOrderedAsyncEnumerable<T> ApplyThenBy<T>(this IOrderedAsyncEnumerable<T> enumerable, List<OrderByDto> orders)
    {
        foreach (var orderBy in orders.Skip(1))
        {
            if (orderBy.Ascending)
            {
                enumerable = enumerable.ThenBy(GetOrderByFunc<T>(orderBy.Property));
            }
            else
            {
                enumerable = enumerable.ThenByDescending(GetOrderByFunc<T>(orderBy.Property));
            }
        }
        return enumerable;
    }

    /// <summary>
    /// Apply order by condition to IEnumerable<Entity>
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="orders">orderby</param>
    /// <returns>IOrderedEnumerable</returns>
    private static IOrderedEnumerable<T> ApplyOrderBy<T>(this IEnumerable<T> enumerable, List<OrderByDto> orders)
    {
        if (orders[0].Ascending)
        {
            return enumerable.OrderBy(GetOrderByFunc<T>(orders[0].Property));
        }
        else
        {
            return enumerable.OrderByDescending(GetOrderByFunc<T>(orders[0].Property));
        }
    }

    /// <summary>
    /// Apply then by condition to IOrderedEnumerable<Entity>
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="orders">orderby</param>
    /// <returns>IOrderedEnumerable</returns>
    private static IOrderedEnumerable<T> ApplyThenBy<T>(this IOrderedEnumerable<T> enumerable, List<OrderByDto> orders)
    {
        foreach (var orderBy in orders.Skip(1))
        {
            if (orderBy.Ascending)
            {
                enumerable = enumerable.ThenBy(GetOrderByFunc<T>(orderBy.Property));
            }
            else
            {
                enumerable = enumerable.ThenByDescending(GetOrderByFunc<T>(orderBy.Property));
            }
        }
        return enumerable;
    }

    #endregion orderBy

    #region publicMethods

    /// <summary>
    /// An extension method for IAsyncEnumerable<Entity>
    /// Apply dynamic filter
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="filter">filter for Where clause, OrderBy, Skip and Take</param>
    /// <returns>IAsyncEnumberable<Entity> with filter applied</returns>
    public static IAsyncEnumerable<T> ApplyFilter<T>(this IAsyncEnumerable<T> enumerable, FilterDto filter)
    {
        enumerable = enumerable.Where(BuildPredicate<T>(filter.Filter));
        if (filter.OrderBy != null && filter.OrderBy.Any())
        {
            enumerable = enumerable.ApplyOrderBy(filter.OrderBy).ApplyThenBy(filter.OrderBy);
        }
        if (filter.Skip.HasValue)
        {
            enumerable = enumerable.Skip(filter.Skip.Value);
        }
        if (filter.Take.HasValue)
        {
            enumerable = enumerable.Take(filter.Take.Value);
        }
        return enumerable;
    }

    /// <summary>
    /// An extension method for IAsyncEnumerable<Entity>
    /// Apply dynamic filter
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="filter">filter only for Where clause</param>
    /// <returns>IAsyncEnumberable<Entity> with filter applied</returns>
    public static IAsyncEnumerable<T> ApplyFilter<T>(this IAsyncEnumerable<T> enumerable, FilterGroupDto filter)
    {
        return enumerable.Where(BuildPredicate<T>(filter));
    }

    /// <summary>
    /// An extension method for IEnumerable<Entity>
    /// Apply dynamic filter
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="filter">filter for Where clause, OrderBy, Skip and Take</param>
    /// <returns>IEnumberable<Entity> with filter applied</returns>
    public static IEnumerable<T> ApplyFilter<T>(this IEnumerable<T> enumerable, FilterDto filter)
    {
        enumerable = enumerable.Where(BuildPredicate<T>(filter.Filter));
        if (filter.OrderBy != null && filter.OrderBy.Any())
        {
            enumerable = enumerable.ApplyOrderBy(filter.OrderBy).ApplyThenBy(filter.OrderBy);
        }
        if (filter.Skip.HasValue)
        {
            enumerable = enumerable.Skip(filter.Skip.Value);
        }
        if (filter.Take.HasValue)
        {
            enumerable = enumerable.Take(filter.Take.Value);
        }
        return enumerable;
    }

    /// <summary>
    /// An extension method for IEnumerable<Entity>
    /// Apply dynamic filter
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <param name="enumerable">enumerable</param>
    /// <param name="filter">filter only for Where clause</param>
    /// <returns>IEnumberable<Entity> with filter applied</returns>
    public static IEnumerable<T> ApplyFilter<T>(this IEnumerable<T> enumerable, FilterGroupDto filter)
    {
        return enumerable.Where(BuildPredicate<T>(filter));
    }

    #endregion publicMethods

    #region validators

    /// <summary>
    /// Checks whether the operator is applicable to the property type
    /// </summary>
    /// <param name="operatorEnum">operatorEnum</param>
    /// <param name="typeEnum">typeEnum</param>
    /// <returns>True; if operator is applicable to the property type, else False;</returns>
    private static bool IsOperatorMatchingType(Operator operatorEnum, PropertyType typeEnum)
    {
        return typeEnum switch
        {
            var x when (x == PropertyType.Short ||
                        x == PropertyType.Int ||
                        x == PropertyType.Long ||
                        x == PropertyType.Double ||
                        x == PropertyType.Decimal) => NumberSpecificOperator(operatorEnum),
            PropertyType.String => CharacterSpecificOperator(operatorEnum),
            PropertyType.DateTime => DateTimeSpecificOperator(operatorEnum),
            PropertyType.Boolean => BooleanSpecificOperator(operatorEnum),
        };
    }

    /// <summary>
    /// Checks whether the given operator is applicable for numbers
    /// </summary>
    /// <param name="op">operator</param>
    /// <returns>True; if the given operator is applicable for numbers</returns>
    private static bool NumberSpecificOperator(Operator op)
    {
        return new List<Operator>()
        {
            Operator.In,
            Operator.NotIn,
            Operator.LessThan,
            Operator.LessThanOrEqual,
            Operator.GreaterThan,
            Operator.GreaterThanOrEqual,
            Operator.Equal,
            Operator.NotEqual,
            Operator.Between,
            Operator.BetweenInclusive,
        }
        .Contains(op);
    }

    /// <summary>
    /// Checks whether the given operator is applicable for text
    /// </summary>
    /// <param name="op">operator</param>
    /// <returns>True; if the given operator is applicable for text</returns>
    private static bool CharacterSpecificOperator(Operator op)
    {
        return new List<Operator>()
        {
            Operator.In,
            Operator.NotIn,
            Operator.Equal,
            Operator.NotEqual,
            Operator.Contains,
            Operator.NotContains,
        }
        .Contains(op);
    }

    /// <summary>
    /// Checks whether the given operator is applicable for DateTime
    /// </summary>
    /// <param name="op">operator</param>
    /// <returns>True; if the given operator is applicable for DateTime</returns>
    private static bool DateTimeSpecificOperator(Operator op)
    {
        return new List<Operator>()
        {
            Operator.In,
            Operator.NotIn,
            Operator.LessThan,
            Operator.LessThanOrEqual,
            Operator.GreaterThan,
            Operator.GreaterThanOrEqual,
            Operator.Equal,
            Operator.NotEqual,
            Operator.Between,
            Operator.BetweenInclusive,
        }
        .Contains(op);
    }

    /// <summary>
    /// Checks whether the given operator is applicable for Boolean
    /// </summary>
    /// <param name="op">operator</param>
    /// <returns>True; if the given operator is applicable for Boolean</returns>
    private static bool BooleanSpecificOperator(Operator op)
    {
        return new List<Operator>()
        {
            Operator.Equal,
            Operator.NotEqual,
        }
        .Contains(op);
    }

    #endregion validators
}
