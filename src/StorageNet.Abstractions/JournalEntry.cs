using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;

namespace StorageNet.Abstractions
{
    public class JournalEntry
    {
        public JournalEntryHeader Header { get; set; }
        public byte[] Content { get; set; }
    }
}
