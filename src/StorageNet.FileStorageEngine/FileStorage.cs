using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StorageNet.Abstractions;

namespace StorageNet.FileStorageEngine
{
    public class FileStorage<V>:InternalStorage, IStorage<V>
    {
        private string _outputFile;

        internal FileStorage(int tableId, string folder) => _outputFile = System.IO.Path.Combine(folder, $"{tableId}.bin");

        public Task Delete(string key) => throw new NotImplementedException();

        public Task<V> Get(string key) => throw new NotImplementedException();

        public Task Put(string key, V value) => throw new NotImplementedException();
    }
}
