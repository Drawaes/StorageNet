using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNet.BTree
{
    public class Node
    {
        public int Key { get; set; }
        public bool IsRoot { get; set; }
        public bool IsLeaf { get; set; }
        public List<Node> Children { get; } = new List<Node>();
    }
}
