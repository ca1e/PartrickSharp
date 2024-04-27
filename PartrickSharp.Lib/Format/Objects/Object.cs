using System.Runtime.InteropServices;

namespace PartrickSharp;

struct CourseObject
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x20)]
    byte[] Reserved;
}