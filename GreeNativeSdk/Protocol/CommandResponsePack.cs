namespace GreeNativeSdk.Protocol
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class CommandResponsePack
    {
        [JsonPropertyName("opt")]
        public List<string> Options { get; set; }

        [JsonPropertyName("p")]
        public List<int> Values { get; set; }
    }
}
