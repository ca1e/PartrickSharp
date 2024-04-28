namespace PartrickSharp;

struct CourseObject
{
    uint XPosition;
    uint YPosition;
    ushort Reserved;
    byte Width;
    byte Height;
    uint Flags;
    uint ChhildFlags;
    uint ExtendedData;
    ushort ID;
    ushort ChihldID;
    ushort LinkID;
    ushort SoundEffectID;
}

struct SoundEffect
{
    byte ID;
    byte XPosition;
    byte YPosition;
    byte Reserved;
}