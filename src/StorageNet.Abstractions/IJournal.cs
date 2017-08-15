using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IJournal :IEnumerable<JournalEntry>, IDisposable
    {
        Task WriteJournalEntry(JournalEntryType type, byte[] content, long id);
        Task WriteJournalEntries(IEnumerable<(JournalEntryType Type, byte[] Content, long TransactionId)> entries);
        Task Open();
    }
}
