using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScimPatch.Converters
{
    public class ObservableCollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var itemType = objectType.GetGenericArguments()[0];
            
            var listType = typeof(List<>).MakeGenericType(itemType);
            var list = serializer.Deserialize(reader, listType);

            var collectionType = typeof(ObservableCollection<>).MakeGenericType(itemType);
            var collection = Activator.CreateInstance(collectionType);

            var addMethod = collectionType.GetMethod("Add")!;

            if (list != default)
                foreach (var item in (IEnumerable<object>)list)
                {
                    addMethod.Invoke(collection, new[] { item });
                }
            
            return collection;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var array = new JArray();
            if (value != default)
                foreach (var item in (IEnumerable<object>)value)
                {
                    array.Add(JToken.FromObject(item, serializer));
                }
            array.WriteTo(writer);
        }
    }
}