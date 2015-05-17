using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
#if DNXCORE50
using System.Reflection;
#endif

namespace Kanapa
{
  public class ViewDefinitionsConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var @object = (IEnumerable<ViewDefinition>) value;
      writer.WriteStartObject();
      foreach (var item in @object)
      {
        writer.WritePropertyName(item.Name);
        writer.WriteStartObject();
        if (item.Mapping != null)
        {
          if (string.IsNullOrEmpty(item.Mapping.Map) == false)
          {
            writer.WritePropertyName("map");
            writer.WriteValue(item.Mapping.Map);
          }
          if (string.IsNullOrEmpty(item.Mapping.Reduce) == false)
          {
            writer.WritePropertyName("reduce");
            writer.WriteValue(item.Mapping.Reduce);
          }
        }
        writer.WriteEndObject();
      }
      writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var obj = JObject.Load(reader);
      return (from item in obj.Properties()
              let name = item.Name
              let @object = obj[item.Name]
              select new ViewDefinition
              {
                Name = name,
                Mapping = new MapReduce
                {
                  Map = @object["map"]?.Value<string>(),
                  Reduce = @object["reduce"]?.Value<string>()
                }
              }
        );
    }

    public override bool CanConvert(Type objectType)
    {
#if DNXCORE50
      var typeInfo = objectType.GetTypeInfo();
      var baseType = typeof(IEnumerable<ViewDefinition>).GetTypeInfo();
      return baseType.IsAssignableFrom(typeInfo);
#else
      return typeof(IEnumerable<ViewDefinition>).IsAssignableFrom(objectType);
#endif
    }
  }
}