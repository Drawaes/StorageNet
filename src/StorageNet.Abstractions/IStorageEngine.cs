
using System;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IStorageEngine
    {
        Task Open(string file);
        Task Close();

        IStorage<K,V> GetStorage<K,V>();

        ValueTask<Buffer<byte>> GetBuffer(int size);

        ValueTask<bool> Put(Buffer<byte> key, Buffer<byte> value);
        ValueTask<Buffer<byte>> Get(Buffer<byte> key);
        ValueTask<bool> Delete(Buffer<byte> key);

    }
}
