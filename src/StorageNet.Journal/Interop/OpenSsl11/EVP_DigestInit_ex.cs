using System;
using System.Runtime.InteropServices;

namespace StorageNet.Interop.OpenSsl11
{
    internal static partial class LibCrypto
    {
        [DllImport(Libraries.LibCrypto, CallingConvention = CallingConvention.Cdecl)]
        private static extern int EVP_DigestInit_ex(EVP_MD_CTX ctx, EVP_HashType type, IntPtr impl);
    }
}
