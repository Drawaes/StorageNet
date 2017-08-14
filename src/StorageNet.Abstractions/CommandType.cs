using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNet.Abstractions
{
    public enum CommandType
    {
        Put,
        Get,
        Delete,
        ChangeSchema
    }
}
