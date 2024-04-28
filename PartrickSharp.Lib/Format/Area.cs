using System.Runtime.InteropServices;

namespace PartrickSharp;

public struct CourseArea
{
    AreaHeader Header;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2600, ArraySubType = UnmanagedType.Struct)]
    CourseObject[] Objects;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 300, ArraySubType = UnmanagedType.Struct)]
    SoundEffect[] SoundEffect;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.Struct)]
    SnakeBlock[] SnakeBlock;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xE420)]
    byte[] ClearPipe; //0x124 * 200
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x348)]
    byte[] PiranhaCreeper; //0x54 * 10
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.Struct)]
    BlockInfo[] ExclamationBlock;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.Struct)]
    BlockInfo[] TrackBlock;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x3E80)]
    byte[] Ground; //0x4 * 4000
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x4650)]
    byte[] Track; //0xC * 1500
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x4B0)]
    byte[] Icicle; //0x4 * 300
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xDBC)]
    byte[] Reserved;
}