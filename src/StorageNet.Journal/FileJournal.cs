using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StorageNet.Abstractions;

namespace StorageNet.Journal
{
    public class FileJournal : IJournal, IEnumerable<JournalEntry>
    {
        private string _directoryLocation;
        private TaskCompletionSource<bool> _currentTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private ConcurrentQueue<(JournalEntry Entry, TaskCompletionSource<bool> Completion)> _waitingEntries = new ConcurrentQueue<(JournalEntry Entry, TaskCompletionSource<bool> Completion)>();
        private FileStream _file;
        private ManualResetEvent _endEvent = new ManualResetEvent(false);
        private Task _writingThread;
        private AutoResetEvent _needsWriting = new AutoResetEvent(false);
        private CancellationTokenSource _cancel = new CancellationTokenSource();
        private byte[] _headerBuffer;
        private byte[] _crc = new byte[4];
        private GCHandle _pin;
        
        public unsafe FileJournal(string directoryLocation)
        {
            _directoryLocation = directoryLocation;
            _headerBuffer = new byte[sizeof(JournalEntryHeader)];
            _pin = GCHandle.Alloc(_headerBuffer, GCHandleType.Pinned);
        }

        public Task Open()
        {
            _file = new FileStream(_directoryLocation, FileMode.Create, FileAccess.Write);
            _writingThread = new Task(WritingLoop, TaskCreationOptions.LongRunning);
            _writingThread.Start();
            return Task.CompletedTask;
        }

        private void WritingLoop()
        {
            while (!_cancel.IsCancellationRequested)
            {
                _needsWriting.WaitOne();
                TaskCompletionSource<bool> task;
                lock (_currentTask)
                {
                    task = _currentTask;
                    _currentTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                }

                while (_waitingEntries.TryPeek(out (JournalEntry Entry, TaskCompletionSource<bool> Task) currentEntry) && currentEntry.Task == task)
                {
                    if (!_waitingEntries.TryDequeue(out currentEntry))
                    {
                        throw new InvalidOperationException();
                    }
                    //Write to disk!!!
                    Marshal.StructureToPtr(currentEntry.Entry.Header, _pin.AddrOfPinnedObject(), false);
                    _file.Write(_headerBuffer, 0, _headerBuffer.Length);
                    _file.Write(currentEntry.Entry.Content, 0, currentEntry.Entry.Content.Length);
                    _file.Write(_crc, 0, 4);
                }
                _file.Flush();
                task.SetResult(true);
            }
            _currentTask.TrySetException(new ObjectDisposedException("The journal has been shutdown so you cannot write to it"));
            _file.Flush();
            _file.Dispose();
            _endEvent.Set();
        }

        public void Dispose()
        {
            _cancel.Cancel();
            _needsWriting.Set();
            _endEvent.WaitOne();
            _pin.Free();
        }

        public Task WriteJournalEntry(JournalEntryType type, byte[] content, long transactionId)
        {
            //This has a race condition
            //The task could be switched while we are adding the item to the queue
            Task task;
            lock (_currentTask)
            {
                task = _currentTask.Task;
                var je = new JournalEntry()
                {
                    Content = content,
                    Header = new JournalEntryHeader() { EntryType = type, Id = transactionId, Size = (uint)content.Length }
                };
                _waitingEntries.Enqueue((je, _currentTask));
            }
            _needsWriting.Set();
            return task;
        }

        public IJournalReader GetReader() => new FileJournalReader(_directoryLocation);

        public Task WriteJournalEntries(IEnumerable<(JournalEntryType Type, byte[] Content, long TransactionId)> entries)
        {
            Task task;
            lock (_currentTask)
            {
                task = _currentTask.Task;
                foreach (var e in entries)
                {
                    var je = new JournalEntry()
                    {
                        Content = e.Content,
                        Header = new JournalEntryHeader() { EntryType = e.Type, Id = e.TransactionId, Size = (uint)e.Content.Length }
                    };
                    _waitingEntries.Enqueue((je, _currentTask));
                }
            }
            _needsWriting.Set();
            return task;
        }

        public IEnumerator<JournalEntry> GetEnumerator() => GetReader();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
