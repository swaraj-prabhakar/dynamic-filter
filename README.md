# Dynamic Filter

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

A package which provides some generic methods for applying filter dynamically along with linq queries.

## Features

- Apply dynamic filter to database linq queries & other list
- Apply dynamic sort to database linq queries & other list

## Installation

Using dotnet cli

```sh
dotnet add package DynamicFilter;
```

## Usage

Add using

```sh
using System.Linq.DynamicFilter;
```

Apply filter on IAsyncEnumerable

```sh
List<User> users = await dbContext.User.AsAsyncEnumerable().ApplyFilter(filter).ToListAsync();
```

Apply filter on IEnumerable

```sh
var filteredList = users.ApplyFilter(filter).ToList();
```

#### Building filters
###### Example filter DTO

```json
{
   "filter":{
      "condition":"Or",
      "filters":[
         {
            "property":"Name",
            "operator":"Contains",
            "value":"t2"
         },
         {
            "property":"Employees",
            "operator":"Any",
            "anyFilter":{
               "condition":"And",
               "filters":[
                  {
                     "property":"FirstName",
                     "operator":"Contains",
                     "value":"m",
                     "caseSensitive": true
                  },
                  {
                     "property":"LastName",
                     "operator":"Contains",
                     "value":"i"
                  }
               ]
            }
         }
      ]
   },
   "orderBy":[
      {
         "property":"Id",
         "ascending":false
      }
   ],
   "skip": 1,
   "take": 10
}
```

- `condition` : Condition by which its child filters are combined. Supported conditions - "**And**", "**Or**"
- `filters` : It should be non-empty, if condition is set.
- `property` : Property name of the entity
- `operator` : Operator name that can be applied on the specified property of the entity. Supported operators - "**Equal**","**NotEqual**","**GreaterThan**","**LessThan**","**GreaterThanOrEqual**","**LessThanOrEqual**","**Between**","**BetweenInclusive**","**In**","**NotIn**","**Contains**","**NotContains**","**Any**"
- `value` : It is a dynamic property. It can be null, if operator is "**Any**". Example values - "*Hello*", *123*, *true*, "*2022-04-06*", *[1,4,6,3]*, *["This","is","a","sample"]*, *["2022-04-06","2000-01-10"]*
- `caseSensitive` : Its default value is **false**. It decides whether case should be considered or not
- `AnyFilter` : Filter that should be applied inside "**Any**" method. It is applicable only if the operator is "**Any**"
- `orderBy` : A list of property names by which the list should be sorted. Sorting will depend on the order of properties specified in this list. It is **optional**
- `ascending` : Default value is **true**.
- `skip` : number of records to be skipped from the ordered list. It is **optional**
- `take` : number of records to be taken from the list. It is **optional**

Example Entities :
---
```cs
public class Employee
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime DateOfJoining { get; set; }
    public bool IsPermanent { get; set; }
    public virtual Department Department { get; set; }
}
```
```cs
public class Department
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public virtual List<Employee> Employees { get; set; }
}
```
###### Sample filters for Employee
---
```json
{
   "filter":{
      "property":"FirstName",
      "operator":"Equal",
      "value":"Marcos"
   }
}
``` 
```json
{
   "filter":{
      "property":"Age",
      "operator":"Equal",
      "value":30
   }
}
```
```json
{
   "filter":{
      "property":"FirstName",
      "operator":"In",
      "value":[
         "Marcos",
         "John"
      ]
   },
   "orderBy":[
      {
         "property":"FirstName"
      }
   ]
}
```
```json
{
   "filter":{
      "condition":"Or",
      "filters":[
         {
            "property":"Id",
            "operator":"Equal",
            "value":2
         },
         {
            "property":"Id",
            "operator":"Equal",
            "value":1
         }
      ]
   },
   "orderBy":[
      {
         "property":"FirstName",
         "ascending":true
      }
   ],
   "skip":0,
   "take":10
}
```
```json
{
   "filter":{
      "property":"Department.Name",
      "operator":"Contains",
      "value":"1",
      "caseSensitive":true
   },
   "orderBy":[
      {
         "property":"FirstName",
         "ascending":true
      }
   ],
   "skip":0,
   "take":10
}
```
###### Sample filters for Department
---
```json
{
   "filter":{
      "property":"Employees",
      "operator":"Any"
   },
   "orderBy":[
      {
         "property":"Id"
      }
   ]
}
```
```json
{
   "filter":{
      "property":"Employees",
      "operator":"Any",
      "anyFilter":{
         "property":"FirstName",
         "operator":"Contains",
         "type":"String",
         "value":"a"
      }
   },
   "orderBy":[
      {
         "property":"Id"
      }
   ]
}
```
```json
{
   "filter":{
      "property":"Employees",
      "operator":"Any",
      "anyFilter":{
         "condition":"And",
         "filters":[
            {
               "property":"FirstName",
               "operator":"Contains",
               "value":"M",
               "caseSensitive":true
            },
            {
               "property":"LastName",
               "operator":"Contains",
               "value":"i"
            }
         ]
      }
   }
}
```
```json
{
   "filter":{
      "condition":"Or",
      "filters":[
         {
            "property":"Employees",
            "operator":"Any",
            "anyFilter":{
               "condition":"And",
               "filters":[
                  {
                     "property":"FirstName",
                     "operator":"Contains",
                     "value":"m"
                  },
                  {
                     "property":"LastName",
                     "operator":"Contains",
                     "value":"i"
                  }
               ]
            }
         },
         {
            "property":"Name",
            "operator":"Contains",
            "value":"t2"
         }
      ]
   },
   "orderBy":[
      {
         "property":"Id",
         "ascending":false
      }
   ]
}
```

## License

MIT

**Free Package**

