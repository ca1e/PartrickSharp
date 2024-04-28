using System.Runtime.InteropServices;

namespace PartrickSharp;

struct SnakeBlock
{
    byte Index;
    byte NodeCount;
    byte Unknown; //1
    byte Reserved;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.Struct)]
    SnakeBlockNode[] Node;
}

struct SnakeBlockNode
{
    ushort Index;
    ushort Direction;
    ushort Unknown;
    ushort Reserved;
}

struct BlockInfo
{
    byte Unknown; //1
    byte Index;
    byte NodeCount;
    byte Reserved;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.Struct)]
    BlockNode[] Node;
}

struct BlockNode
{
    byte Unknown; //1
    byte Direction;
    ushort Reserved;
}