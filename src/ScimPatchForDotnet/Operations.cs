using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ScimPatchForDotnet
{
    public partial class Operations
    {
        internal static IList<Operation> FromJson(string json) =>
            JsonConvert.DeserializeObject<List<Operation>>(json, Operations.Converter.Settings) 
            ?? throw new InvalidOperationException();

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
            };
        }
    }
}