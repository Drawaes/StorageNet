using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StorageNet.Abstractions
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JournalEntryHeader
    {
        public JournalEntryType EntryType { get; set; }
        public long Id { get; set; }
        public uint Size { get; set; }
    }
}
