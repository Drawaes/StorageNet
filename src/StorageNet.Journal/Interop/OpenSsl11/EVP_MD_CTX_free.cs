using System;
using System.Runtime.InteropServices;

namespace StorageNet.Interop.OpenSsl11
{
    internal static partial class LibCrypto
    {
        [DllImport(Libraries.LibCrypto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void EVP_MD_CTX_free(IntPtr ctx);
    }
}
