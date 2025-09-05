using System;

namespace {
  public class Config {
    public string Host { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string DataBAse { get; set; }

    public static Config LoadFromFile(string filePath) {
      try {
        string json = File.ReadAllText(filePath);

        return JsonConvert.DeserializeObject<Config>(json);
      }
      catch (Exception ex) {
        Console.WriteLine("Ошибка загрузки конфига: " + ex.Message);

        return new Config();
      }
    }
  }
}
