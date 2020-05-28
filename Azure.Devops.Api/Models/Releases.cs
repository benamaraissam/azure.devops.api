using Newtonsoft.Json;
using System.Collections.Generic;

namespace Azure.Devops.Api.Models
{
    public class Releases
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public IList<Release> Value { get; set; }
    }
}
