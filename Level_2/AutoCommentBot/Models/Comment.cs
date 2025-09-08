using NEwtonsoft.Json;

namespace AutoCommentBot.Models {
  public class Comment {
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("rating")]
    public int? Rating { get; set; }
  }
}
