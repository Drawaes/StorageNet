using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNet.Abstractions
{
    public interface IJournalReader:IEnumerator<JournalEntry>
    {
    }
}
