using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using StorageNet.Abstractions;
using StorageNet.Journal;

namespace StorageNet.HashTable
{
    public class Engine : IDisposable
    {
        private IJournal _journal;
        private PipeFactory _pipeFactory;
        private IPipe _journalPipe;

        public Engine(string folder)
        {
            _journal = new FileJournal(folder);
            _journal.Open();
            _pipeFactory = new PipeFactory();
            _journalPipe = _pipeFactory.Create();
        }

        public void Dispose()
        {
            _journal?.Dispose();
            _journal = null;
        }
    }
}
