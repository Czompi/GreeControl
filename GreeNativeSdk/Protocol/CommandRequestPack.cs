namespace GreeNativeSdk.Protocol
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class CommandRequestPack : RequestPackInfo
    {
        [JsonPropertyName("opt")]
        public List<string> Options { get; set; }

        [JsonPropertyName("p")]
        public List<int> Values { get; set; }

        public static CommandRequestPack Create(string clientId, List<string> columns, List<int> values)
        {
            return new CommandRequestPack()
            {
                Type = "cmd",
                MacAddress = clientId,
                Options = columns,
                Values = values,
                UniqueId = null
            };
        }
    }
}
