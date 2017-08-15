using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IStorage<V>
    {
        Task Put(string key, V value);
        Task<V> Get(string key);
        Task Delete(string key);
    }
}
