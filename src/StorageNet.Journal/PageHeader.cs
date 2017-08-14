using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StorageNet.Journal
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct PageHeader:IEquatable<PageHeader>
    {
        public uint Length;
        public uint ActualLength;
        public fixed byte CheckSum[32];
                
        public bool Equals(PageHeader other)
        {
            if (Length != other.Length)
            {
                return false;
            }
            if (ActualLength != other.ActualLength)
            {
                return false;
            }
            fixed (void* check1Ptr = CheckSum)
            {
                var s1 = new Span<byte>(check1Ptr, 32);
                var s2 = new Span<byte>(other.CheckSum, 32);
                return s1.SequenceEqual(s2);
            }
        }
    }
}
