namespace GreeNativeSdk.Protocol
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class DeviceStatusRequestPack : RequestPackInfo
    {
        [JsonPropertyName("cols")]
        public List<string> Columns { get; set; }

        public static DeviceStatusRequestPack Create(string clientId, List<string> columns)
        {
            return new DeviceStatusRequestPack()
            {
                Type = "status",
                MacAddress = clientId,
                Columns = columns,
                UniqueId = null
            };
        }
    }
}