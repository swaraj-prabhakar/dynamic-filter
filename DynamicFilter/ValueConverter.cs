// <copyright file="ValueConverter.cs"> 
// Copyright (c) 2022 All Rights Reserved 
// <author>Swaraj P P</author> 
// </copyright>

using Newtonsoft.Json;

namespace DynamicFilter;

/// <summary>
/// ValueConverter
/// </summary>
internal class ValueConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            List<dynamic> arr = new List<dynamic>();
            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                arr.Add(reader.Value);
            }
            return arr;
        }

        return reader.Value;
    }

    public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
