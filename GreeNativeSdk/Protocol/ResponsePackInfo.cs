namespace GreeNativeSdk.Protocol
{
    using System.Text.Json.Serialization;

    public class ResponsePackInfo : PackInfo
    {
        [JsonPropertyName("uid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? UID { get; set; }

        [JsonPropertyName("tcid")]
        public string TargetClientId { get; set; }

        [JsonPropertyName("pack")]
        public string Pack { get; set; }
    }
}
