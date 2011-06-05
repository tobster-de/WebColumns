
namespace WebColumns.Logic
{
    public enum ElementColor : byte
    {
        Red = 0,
        Green,
        Blue,
        Yellow,
        Violet,
        //Orange
    }

    public enum Orientation : byte
    {
        Vertical = 0,
        Horizontal = 1
    }

    public enum BoardMode
    {
        ElementMove,
        LinkChecking,
        CheckingMove
    }

}