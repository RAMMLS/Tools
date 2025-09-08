using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoCommentBot.Config {
  public class BotCOnfig {
    [JsonProperty("websiteUrl")]
    public string WebsiteUrl{ get; set; }

    [JsonProperty("commentSelector")]
    public string CommentSelector { get; set; }

    [JsonProperty ("submitButtonSelector")]
    public string  SubmitButtonSelector { get; set; }

    [JsonProperty("comments")]
    public string List<Comment> Comments { get; set; }

    [JsonProperty("userProfiles")]
    public string List<UserProfile> UserProfile { get; set; }

    [JsonProperty("delayBetweenCommentSeconds")]
    public int DelayBetweenCommentSeconds { get; set; }

    [JsonProperty("maxRandomDelaySeconds")]
    public int MaxRandomDelaySeconds { get; set; }

    [JsonProperty ("headlessMode")]
    public bool HeadlessMode { get; set; }
  }
}
