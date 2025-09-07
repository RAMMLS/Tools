using System.Collections.Generic;

namespace SimpleGoogleBot
{
    public class DirectoryNode : FileSystemNode
    {
        public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>();

        public DirectoryNode(string name, DirectoryNode parent) : base(name, (parent != null ? System.IO.Path.Combine(parent.FullPath, name) : name), parent)
        {
        }
    }
}

