using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNet.FileStorageEngine
{
    public class StorageBlock
    {
        private byte[] _storage = new byte[1024 * 1024 * 4];
        private StorageBlock _leftBlock;
        private StorageBlock _rightBlock;

        private string _midKey;


    }
}
