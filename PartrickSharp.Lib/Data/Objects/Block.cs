using System.Runtime.InteropServices;

namespace PartrickSharp;

public struct SnakeBlock
{
    public byte Index;
    public byte NodeCount;
    byte Unknown; //1
    byte Reserved;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.Struct)]
    public SnakeBlockNode[] Node;
}

public struct SnakeBlockNode
{
    public ushort Index;
    public ushort Direction;
    ushort Unknown;
    ushort Reserved;
}

public struct BlockInfo
{
    byte Unknown; //1
    public byte Index;
    public byte NodeCount;
    byte Reserved;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.Struct)]
    public BlockNode[] Node;
}

public struct BlockNode
{
    byte Unknown; //1
    public byte Direction;
    ushort Reserved;
}