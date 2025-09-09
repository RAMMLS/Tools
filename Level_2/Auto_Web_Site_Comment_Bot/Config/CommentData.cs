using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoCommentBot.Config {
  public class CommentData {
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    [JsonProperty("rating")]
    public int? Rating { get; set; }
  }
}
