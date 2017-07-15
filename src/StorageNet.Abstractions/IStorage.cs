using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IStorage<K,V>
    {
        ValueTask<K> Put(K key, V value);
        ValueTask<V> Get(K key);
        ValueTask<K> Delete(K key);
    }
}
