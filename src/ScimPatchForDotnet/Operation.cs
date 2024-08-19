using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ScimPatchForDotnet
{
    public partial class Operation
    {
        [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
        public string? From { get; set; }

        [JsonProperty("op", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OperationType OperationType { get; set; }

        [JsonProperty("path", Required = Required.Always)]
        public string Path { get; set; } = null!;

        [JsonProperty("value")]
        public JToken? Value { get; set; }
    }
}