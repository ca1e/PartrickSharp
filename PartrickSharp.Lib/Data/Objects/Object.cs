namespace PartrickSharp;

public struct CourseObject
{
    public uint XPosition;
    public uint YPosition;
    ushort Reserved;
    byte Width;
    byte Height;
    uint Flags;
    uint ChhildFlags;
    uint ExtendedData;
    public ushort ID;
    ushort ChihldID;
    ushort LinkID;
    ushort SoundEffectID;
}

public struct SoundEffect
{
    public  byte ID;
    public byte XPosition;
    public byte YPosition;
    byte Reserved;
}