using System.Runtime.InteropServices;

namespace PartrickSharp;

public struct CourseHeader
{
    public byte StartY;
    public byte GoalY;
    ushort GoalX;
    public ushort Timer;
    ushort CCA; //Clear Condition Amount
    ushort LSYear; //Last Saved Year
    byte LSMonth;
    byte LSDay;
    byte LSHour;
    byte LSMinute;
    byte AutoScrollSpeed; //Custom Autoscroll Speed
    byte ClearConditionCat; //Clear Condition Category
    uint ClearConditionCRC32;
    uint GameVersion;
    uint ManagementFlags;
    uint ClearCheckAttempts;
    uint ClearCheckTime;
    uint CreationID;
    public ulong UploadID;
    uint ClearCheckGameVer; //Clear Check Game Version
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xBC)]
    byte[] Reserved;
    byte Unknown; //usually 0xFF
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x3)]
    byte[] GameStyle;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0x42)]
    byte[] Name;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst =0xCA)]
    byte[] Description;
}

public struct AreaHeader
{
    byte Theme;
    byte AutoscrollType;
    byte BoundaryFlags;
    byte Orientation;
    byte LiquidMode;
    byte LiquidSpeed;
    byte StartLiquidHeight;
    uint RightBoundary;
    uint TopBoundary;
    uint LeftBoundary;
    uint BottomBoundary;
    uint Flags;
    uint ObjectCount;
    uint SoundEffectCound;
    uint SnakeBlockCount;
    uint ClearPipeCound;
    uint PiranhhaCreeperCount;
    uint ExclamationBlockCount;
    uint TrackBlockCount;
    uint Reserved;
    uint GroundCount;
    uint TrackCount;
    uint LcicleCount;
}