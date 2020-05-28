using Newtonsoft.Json;
using System.Collections.Generic;

namespace Azure.Devops.Api.Models
{
    public class Release
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("variables")]
        public Dictionary<string, VariableValue> Variables { get; set; }
    }
}
