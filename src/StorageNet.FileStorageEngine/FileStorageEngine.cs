using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StorageNet.Abstractions;
using StorageNet.Journal;

namespace StorageNet.FileStorageEngine
{
    public class FileStorageEngine : IStorageEngine
    {
        private string _folder;
        private IJournal _journal;
        private Dictionary<Type, InternalStorage> _storageMap = new Dictionary<Type, InternalStorage>();
        private Dictionary<Type, int> _storageIdMap = new Dictionary<Type, int>();
        private int _nextTableId = 0;
        private long _lastTableTransaction = 0;
        private long _opId;

        public FileStorageEngine(string folder)
        {
            _folder = folder;
            _journal = new FileJournal(Path.Combine(folder, "journal.bin"));
        }

        public Task Open()
        {
            long lastId = 0;
            foreach (var je in _journal)
            {
                if (je.Header.EntryType != JournalEntryType.EndOfFile)
                {
                    lastId = je.Header.Id;
                }
            }
            _opId = lastId;
            return _journal.Open();
        }

        public Task Close() => WriteStorageMap();
       
        public Task<IStorage<V>> CreateStorage<V>()
        {
            lock (_storageMap)
            {
                var transactionId = System.Threading.Interlocked.Increment(ref _opId);
                _lastTableTransaction = transactionId;
                var tableType = typeof(V);
                var storage = new FileStorage<V>(_nextTableId, _folder);
                _storageMap.Add(tableType, storage);
                _storageIdMap.Add(tableType, _nextTableId);
                _nextTableId++;
                return
                    _journal.WriteJournalEntry(JournalEntryType.CreateTable,
                    System.Text.Encoding.UTF8.GetBytes(tableType.ToString()), transactionId)
                    .ContinueWith(_ => (IStorage<V>)storage);
            }
        }

        private async Task WriteStorageMap()
        {
            (Type, int)[] _tables;
            long tableTransaction;
            lock (_storageMap)
            {
                _tables = _storageIdMap.Select(kv => (kv.Key, kv.Value)).ToArray();
                tableTransaction = _lastTableTransaction;
            }
            using (var fs = new FileStream(Path.Combine(_folder, "tables.txt"), FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(fs))
            {
                await writer.WriteLineAsync("------------------");
                await writer.WriteLineAsync(tableTransaction.ToString());
                for (var i = 0; i < _tables.Length; i++)
                {
                    await writer.WriteLineAsync($"{_tables[i].Item2} {_tables[i].Item1.ToString()}");
                }
                await writer.WriteLineAsync("FINISHED");
            }
        }
        
        public Task<IStorage<V>> GetStorage<V>()
        {
            var tableType = typeof(V);
            lock (_storageMap)
            {
                if (_storageMap.TryGetValue(tableType, out InternalStorage value))
                {
                    return Task.FromResult((IStorage<V>)value);
                }
                throw new InvalidOperationException("Could not find table");
            }
        }
    }
}
