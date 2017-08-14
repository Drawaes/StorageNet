using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNet.Abstractions
{
    [Flags]
    public enum JournalEntryType : byte
    {
        CreateTable = 0x00,
        DeleteTable = 0x01,
        UpdateRecord = 0x02,
        DeleteRecord = 0x03,
        InsertRecord = 0x04,
        UpsertRecord = 0x05,
        EndOfFile = 0x06,
        Checkpoint = 0x07,


        Compressed = 0x08,
    }
}
