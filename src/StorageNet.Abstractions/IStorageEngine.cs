
using System;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IStorageEngine
    {
        Task Open();
        Task Close();

        Task<IStorage<V>> CreateStorage<V>();
        Task<IStorage<V>> GetStorage<V>();
    }
}
