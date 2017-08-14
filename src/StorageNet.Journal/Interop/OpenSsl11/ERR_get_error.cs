using System.Runtime.InteropServices;

namespace StorageNet.Interop.OpenSsl11
{
    internal static partial class LibCrypto
    {
        [DllImport(Libraries.LibCrypto, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ERR_get_error();
    }
}
