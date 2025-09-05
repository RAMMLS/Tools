using System;
using System.Threading.Tasks;

namespace FtpUserFootprint {
  class Program {
    static async Task Main (string[] args) {
      try {
        var config = Configuration.LoadConfiguration();
        using (var ftpCLient = new FtpClient(config.FtpServer, config.FtpUsername, config.Password)) {
          await ftpClient.ConnectAsync();

          var files = await ftpClient.ListFilesAsync("/");

          Console.WriteLine("Files on FTP Server: ");

          foreach (var file in files) {
            Console.WriteLine($"- {file.Name} (Size: {file.Size} bytes)");
          }
          //Disconnect
          ftpCLient.Disconnect();
        }
      }
      catch (Exception ex) {
        Console.WriteLine($"An error occured: {ex.Message}");
      }

      Console.WriteLine("Press any key to exit.");
      Console.ReadKey();
    }
  }
}
