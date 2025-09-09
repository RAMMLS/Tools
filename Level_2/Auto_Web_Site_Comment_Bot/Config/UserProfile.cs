using Newtonsoft.Json;

namespace AutoCommentBot.Config {
  public class UserProfile {
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Passeord { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }
}
