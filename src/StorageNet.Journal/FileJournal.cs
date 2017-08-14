using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StorageNet.Abstractions;

namespace StorageNet.Journal
{
    public class FileJournal : IJournal
    {
        private string _directoryLocation;
        private TaskCompletionSource<bool> _currentTask = new TaskCompletionSource<bool>();
        private BlockingCollection<(JournalEntry Entry, TaskCompletionSource<bool> Completion)> _waitingEntries = new BlockingCollection<(JournalEntry Entry, TaskCompletionSource<bool> Completion)>();
        private FileStream _file;
        private ManualResetEvent _endEvent = new ManualResetEvent(false);

        public FileJournal(string directoryLocation) => _directoryLocation = directoryLocation;

        public Task Open()
        {
            _file = new FileStream(_directoryLocation, FileMode.Create, FileAccess.Write);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _waitingEntries.CompleteAdding();
            _endEvent.WaitOne();
        }

        public Task WriteJournalEntry(IEnumerable<JournalEntry> entries) => throw new NotImplementedException();

        public IJournalReader GetReader() => new FileJournalReader(_directoryLocation);
    }
}
