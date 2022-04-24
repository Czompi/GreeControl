namespace GreeNativeSdk.Protocol
{
    using System.Text.Json.Serialization;

    // TODO inherit from response pack info header or something
    public class BindResponsePack
    {
        [JsonPropertyName("mac")]
        public string MAC { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
