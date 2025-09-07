namespace SimpleGoogleBot {
  public abstract class FileSystemNode {
    public string Name { get; set; }
    public string FullPath { get; set; }

    public FileSystemNode Parent { get; set; }

    public FileSystemNode(string name, string fullPath, FileSystemNode parent) {
      Name = name;
      FullPath = fullPath;
      Parent = parent;
    }
  }
}
