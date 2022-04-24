namespace GreeNativeSdk.Protocol
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class DeviceStatusResponsePack
    {
        [JsonPropertyName("cols")]
        public List<string> Columns { get; set; }

        [JsonPropertyName("dat")]
        public List<int> Values { get; set; }
    }
}