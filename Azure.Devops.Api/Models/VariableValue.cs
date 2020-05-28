using Newtonsoft.Json;

namespace Azure.Devops.Api.Models
{
    public class VariableValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
