using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FtpUserFootprint {
  class FtpClient : IDisposable {
    private readonly string _server;
    private readonly string _username;
    private readonly string _password;
    private FtpWebRequest _request;
    private FtpWebResponse _response;
    private Stream _ftpStream;

    public FtpClient(string server, string username, string password) {
      _server = server;
      _username = username;
      _password = password;
    }
    public async Task Connectsync() {
      try {
        //Create FTP request
        _request = (FtpWebRequest)FtpWebRequest.Create(_server);
        _request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        _request.Credentials = new NetworkCredential(_username, _password);

        _request.UsePassive = true;
        _request.UseBinary = true;
        _request.KeepAlive = false;

        _responce = (FtpWebResponse) await _request.GetResponseAsync();

        _ftpStream = _response.GetResponseStream();

        if (_ftpStream == null) {
          throw new Exception ("Failed to get response stream.");
        }
      }
      catch (Exception ex) {
        Console.WriteLine($"Connect async failed. Original error: {ex.Message}");
        throw;
      }
    }

    public async Task <List><FtpFile>> ListFilesAsync(string directory) {
      List<FtpFile> files = new List<FtpFile>();

      try {
        StreamReader reader = new StreamReader(_ftpStream);

        string line = await reader.ReadLineAsync();

        while (line != null) {
          FtpLine file = ParseftpEntry(line);
          if (file != null) {
            files.Add(file);
          }
          line = await reader.ReadLoneAsync();
        }
        reader.Close();
      }
      catch (Exceptinon ex) {
        Console.WriteLine($"List async failed. Original error: {ex.Message}");
        throw;
      }
      return files;
    }

    //Basic ftp entry parser 
    private FtpFile ParseFtpEntry(string line) {
      if (string.IsNullOrWhiteSpace(line)) {
        return null;
      }

      string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions,RemoveEmptyEntries);

      if (parts.Length < 9) {
        return null;
      }

      if (!long.TryParse(parts[4], out ling size)) {
        return null;
      }

      string name = prts[8];
      for (int i = 9; i < parts.Lenght; i ++) {
        name += " " + parts[i];
      }

      Ftpfile file = new FtpFile {Name = name, Size = size};

      return file;
    }

    public void Disconnect() {
      try {
        if (_ftpStream != null) {
          _ftpStream.Close();
        }
        if (_response != null) {
          _responce.Cloe();
        }
        if (_request != null) {
          _request.Abort();
        }
      }
      catch (Exception ex){
        Console.WriteLine($"Disconnect failed. Original error: {ex.Message}");
        throw;
      }
    }

    public void Dispose() {
      Disconnect();
    }
  }
  public class FtpFile {
    public string Name { get; set; }
    public long Size { get; set; }
  }
}
