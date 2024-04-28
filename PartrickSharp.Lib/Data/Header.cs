using System.Runtime.InteropServices;

namespace PartrickSharp;

public struct CourseHeader
{
    public byte StartY;
    public byte GoalY;
    public ushort GoalX;
    public ushort Timer;
    public ushort CCA; //Clear Condition Amount
    public ushort LSYear; //Last Saved Year
    public byte LSMonth;
    public byte LSDay;
    public byte LSHour;
    public byte LSMinute;
    public byte AutoScrollSpeed; //Custom Autoscroll Speed
    public byte ClearConditionCat; //Clear Condition Category
    public uint ClearConditionCRC32;
    public uint GameVersion;
    public uint ManagementFlags;
    public uint ClearCheckAttempts;
    public uint ClearCheckTime;
    public uint CreationID;
    public ulong UploadID;
    public uint ClearCheckGameVer; //Clear Check Game Version
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xBC)]
    byte[] Reserved;
    byte Unknown; //usually 0xFF
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x3)]
    public byte[] GameStyle;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x42)]
    public byte[] Name;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xCA)]
    public byte[] Description;
}

public struct AreaHeader
{
    public byte Theme;
    public byte AutoscrollType;
    public byte BoundaryFlags;
    public byte Orientation;
    public byte LiquidMode;
    public byte LiquidSpeed;
    public byte StartLiquidHeight;
    public uint RightBoundary;
    public uint TopBoundary;
    public uint LeftBoundary;
    public uint BottomBoundary;
    public uint Flags;
    public uint ObjectCount;
    public uint SoundEffectCound;
    public uint SnakeBlockCount;
    public uint ClearPipeCound;
    public uint PiranhhaCreeperCount;
    public uint ExclamationBlockCount;
    public uint TrackBlockCount;
    uint Reserved;
    public uint GroundCount;
    public uint TrackCount;
    public uint LcicleCount;
}