using Microsoft.Json;
using System.Collections.Generic;

namespace AutoCommentBot.Config {
  public class BotConfig {
    [JsonProperty("websiteConfigs")]
    public List<WebsiteConfig> WebsiteConfigs { get; set; }

    [JsonProperty("userProfiles")]
    public List <UserProfile> UserProfiles { get; set; }

    [JsonProperty("defaultDelayBetweenCommentsSeconds")]
    public int DefaultDelayBetweenCommentsSeconds { get; set; }

    [JsonProperty("useRandomDelays")]
    public bool UserRAndomDelaySeconds { get; set; }

    [JsonProperty("maxRandomDelaySeconds")]
    public int MaxRandomDelaySeconds { get; set; }

    [JsonProperty("headlessMode")]
    public bool HeadlessMode { get; set; }

    [JsonProperty("proxyEnabled")]
    public bool ProxyEnabled { get; set; }

    [JsonProperty("useCaptchaSolving")]
    public bool UseCaptchaSolving { get; set; }

    [JsonProperty("commentingStrategy")]
    public string CommentingStrategy { get; set; }
  }
}
