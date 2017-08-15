using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using StorageNet.Abstractions;
using Xunit;

namespace StorageNet.Journal.Test
{
    public class WriteAndReadJournalFacts
    {
        [Fact]
        public async Task WriteTwoJournalBlocks()
        {
            var guid = Guid.NewGuid();
            var journal = new FileJournal($"./{guid.ToString()}.txt");
            await journal.Open();
            var entries = new(JournalEntryType Type, byte[] Content, long TransactionId)[]
            {
                (JournalEntryType.CreateTable, new byte[100], 0),
                (JournalEntryType.DeleteRecord, new byte[244], 1)
            };
            await journal.WriteJournalEntries(entries);
            journal.Dispose();
            var results = journal.ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal(JournalEntryType.EndOfFile, results[2].Header.EntryType);
            Assert.Equal(JournalEntryType.CreateTable, results[0].Header.EntryType);
            Assert.Equal(JournalEntryType.DeleteRecord, results[1].Header.EntryType);
            Assert.Equal(100, (int)results[0].Header.Size);
            Assert.Equal(244, (int)results[1].Header.Size);
        }

        [Fact]
        public async Task WriteALotOfRecords()
        {
            var guid = Guid.NewGuid();
            var journal = new FileJournal($"./{guid.ToString()}.txt");
            await journal.Open();
            var list = new List<Task>();
            for (var i = 0; i < 500; i++)
            {
                list.Add(journal.WriteJournalEntry(JournalEntryType.InsertRecord, new byte[1000], 100));
            }
            await Task.WhenAll(list);

            var distCount = list.Distinct().Count();
            Assert.NotEqual(list.Count, distCount);
            journal.Dispose();
            var records = journal.ToArray();
            Assert.Equal(list.Count + 1, records.Length);
        }
    }
}
