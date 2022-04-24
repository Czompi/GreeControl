namespace GreeNativeSdk.Protocol
{
    using System.Text.Json.Serialization;

    public class RequestPackInfo
    {
        [JsonPropertyName("t")]
        public string Type { get; set; }

        [JsonPropertyName("uid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? UniqueId { get; set; }

        [JsonPropertyName("mac")]
        public string MacAddress { get; set; }
    }
}
