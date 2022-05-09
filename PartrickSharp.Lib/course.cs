struct Course
{
    byte StartY;
    byte GoalY;
    ushort GoalX;
    ushort Timer;
    ushort CCA; //Clear Condition Amount
    ushort LSYear; //Last Saved Year
    byte LSMonth;
    byte LSDay;
    byte LSHour;
    byte LSMinute;
    byte AutoScrollSpeed;
    byte ClearConditionCat;
    uint ClearConditionCRC32;
    uint GameVersion;
    uint ManagementFlags;
    uint ClearCheckAttempts;
    uint ClearCheckTime;
    uint CreationID;
    ulong UploadID;
    uint ClearCheckGameVer;
    // 0xBC
    byte[] Reserved;
    byte unknown; // usually 0xFF
    // 0x3
    byte[] GameStyle;
    // x42
    byte[] Name;
    // 0xCA
    byte[] Description;
}