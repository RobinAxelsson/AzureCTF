using Newtonsoft.Json;

namespace CTF_shared
{
    public class Answer
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
