using System.Runtime.InteropServices;

namespace PartrickSharp;

public class CourseSMM2
{
    public static T BytesToStructure<T>(byte[] databuffer)
    {
        object structure = null;
        int size = Marshal.SizeOf(typeof(T));
        IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(databuffer, 0, allocIntPtr, size);
            structure = Marshal.PtrToStructure(allocIntPtr, typeof(T));
        }
        finally
        {
            Marshal.FreeHGlobal(allocIntPtr);
        }
        return (T)structure;
    }
}