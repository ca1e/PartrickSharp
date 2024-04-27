using System.Runtime.InteropServices;

namespace PartrickSharp;

struct CourseArea
{
    AreaHeader Header;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2600, ArraySubType = UnmanagedType.Struct)]
    CourseObject[] Objects;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x4B0)]
    byte[] SoundEffect; //0x4 * 300
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x12D4)]
    byte[] SnakeBlock; //0x3C4 * 5
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xE420)]
    byte[] ClearPipe; //0x124 * 200
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x348)]
    byte[] PiranhaCreeper; //0x54 * 10
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x1B8)]
    byte[] ExclamationBlock; // !Block 0x2C * 10
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x1B8)]
    byte[] TrackBlock; //0x2C * 10
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x3E80)]
    byte[] Ground; //0x4 * 4000
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x4650)]
    byte[] Track; //0xC * 1500
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x4B0)]
    byte[] Icicle; //0x4 * 300
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xDBC)]
    byte[] Reserved;
}