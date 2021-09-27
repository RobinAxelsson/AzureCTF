using Newtonsoft.Json;

namespace LinkSpace
{
    public class Link
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } //URL, "Id" required
        public string Name { get; set; }
        public string Group { get; set; }
        public string[] Tags { get; set; }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}

