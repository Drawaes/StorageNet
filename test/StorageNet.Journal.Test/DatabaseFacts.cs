using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StorageNet.Journal.Test
{
    public class DatabaseFacts
    {
        [Fact]
        public async Task OpenAndCreateTable()
        {
            var db = new FileStorageEngine.FileStorageEngine("C:\\code\\db");
            await db.Open();
            var storage = await db.CreateStorage<string>();
            await db.Close();
        }
    }
}
