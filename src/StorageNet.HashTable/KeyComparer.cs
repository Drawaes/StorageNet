using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNet.HashTable
{
    public class KeyComparer:IComparer<Span<byte>>
    {
        public int Compare(Span<byte> firstKey, Span<byte> secondKey)
        {
            if(firstKey.Length != secondKey.Length)
            {
                //we have different length keys
                var smallest = Math.Min(firstKey.Length, secondKey.Length);
                for(var i = 0; i < smallest;i++)
                {
                    if (firstKey[i] > secondKey[i])
                    {
                        return 1;
                    }
                    else if (secondKey[i] > firstKey[i])
                    {
                        return -1;
                    }
                }
                return firstKey.Length > secondKey.Length ? 1 : -1;
            }
            else
            {
                if(firstKey.SequenceEqual(secondKey))
                {
                    return 0;
                }
                //Same length but not equal
                for(var i = 0; i < firstKey.Length;i++)
                {
                    if(firstKey[i] > secondKey[i])
                    {
                        return 1;
                    }
                    if(secondKey[i] > firstKey[i])
                    {
                        return -1;
                    }
                }
                throw new InvalidOperationException("How did we get there?");
            }
        }
    }
}
