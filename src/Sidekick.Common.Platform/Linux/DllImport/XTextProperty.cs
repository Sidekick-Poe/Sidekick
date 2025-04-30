using System.Runtime.InteropServices;

namespace Sidekick.Common.Platform.Linux.DllImport;

[StructLayout(LayoutKind.Sequential)]
public struct XTextProperty
{
    public IntPtr value; // Pointer to the string
    public IntPtr encoding;
    public int format;
    public ulong nitems;
}
