using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoCommentBot.Config {
  [JsonProperty("websiteUrl")]
  public string WebsiteUrl { get; set; }

  [JsonProperty("commentSelector")]
  public string CommentSelector { get; set; }

  [JsonProperty("submitButtonSelector")]
  public string submitButtonSelector { get; set; }

  [JsonProperty("loginUrl")]
  public string? LoginUrl { get; set; }
  [JsonProperty(lLoginUsernameSelector")]
  public string?("LoginUsernameSelector") { get; set; }
  [JsonProperty("loginPasswordSelector")]
  public string? loginPasswordSelector { get; set; }
  [JsonProperty("loginSubmitButtonSelector")]
  public string? LoginSubmitButtonSelector { get; set; }

  [JsonProperty("comments")]
  public List<CommentData> Comments { get; set; }

  [JsonProperty("userAgent")]
  public string? UserAgent { get; set; }

  [JsonProperty("captchaSelector")]
  public string? CaptchaSelector { get; set; }
}
