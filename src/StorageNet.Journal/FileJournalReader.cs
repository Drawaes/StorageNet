using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using StorageNet.Abstractions;

namespace StorageNet.Journal
{
    public class FileJournalReader : IJournalReader
    {
        private FileStream _fileStream;
        private string _location;
        private byte[] _headerBuffer;
        private JournalEntry _currentEntry;

        public unsafe FileJournalReader(string location)
        {
            _location = location;
            _fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
            _headerBuffer = new byte[sizeof(JournalEntryHeader)];
        }

        private void Open() => _fileStream = new FileStream(_location, FileMode.Open, FileAccess.Read);

        public JournalEntry Current => throw new NotImplementedException();

        object IEnumerator.Current => Current;

        public void Dispose() => _fileStream.Dispose();

        public unsafe bool MoveNext()
        {
            if (_currentEntry?.Header.EntryType == JournalEntryType.EndOfFile)
            {
                return false;
            }
            var read = _fileStream.Read(_headerBuffer, 0, _headerBuffer.Length);
            if(read == 0)
            {
                _currentEntry = new JournalEntry()
                {
                    Content = null,
                    Header = new JournalEntryHeader()
                    {
                        EntryType = JournalEntryType.EndOfFile
                    }
                };
                return true;
            }
            if (read != _headerBuffer.Length)
            {
                throw new InvalidDataException();
            }
            JournalEntryHeader header;
            fixed (void* ptr = _headerBuffer)
            {
                header = Unsafe.Read<JournalEntryHeader>(ptr);
            }
            var content = new byte[header.Size];
            var offset = 0;
            var count = content.Length;

            while (count > 0)
            {
                read = _fileStream.Read(content, offset, count);
                if (read == 0)
                {
                    throw new InvalidOperationException();
                }
                offset += read;
                count -= read;
            }

            var crc = new byte[4];
            offset = 0;
            count = 4;
            while (count > 0)
            {
                read = _fileStream.Read(crc, offset, count);
                if (read == 0)
                {
                    throw new InvalidOperationException();
                }
                offset += read;
                count -= read;
            }

            //todo need to check crc

            _currentEntry = new JournalEntry()
            {
                Header = header,
                Content = content
            };

            return true;
        }
                
        public void Reset()
        {
            Dispose();
            Open();
        }
    }
}
