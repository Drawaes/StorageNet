using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IJournal : IDisposable
    {
        Task WriteJournalEntry(IEnumerable<JournalEntry> entries);
        Task Open();
    }
}
