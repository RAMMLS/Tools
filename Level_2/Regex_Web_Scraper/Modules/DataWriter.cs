using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extentions.Logging;
using RegexWebScrapper.Models;

namespace RegexWebScrapper.Modules {
  public interface IDataWriter {
    void WriteDataToFile(List<ScrapedItem> data, string filePath);
  }
}
