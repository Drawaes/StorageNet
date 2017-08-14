using System;
using System.Runtime.InteropServices;

namespace StorageNet.Interop.OpenSsl11
{
    internal static partial class LibCrypto
    {
        [DllImport(Libraries.LibCrypto, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void ERR_error_string_n(int e, byte* buf, UIntPtr len);
    }
}
