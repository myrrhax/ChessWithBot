using ChessWithBot.Game.Pieces;
using System.Windows.Media;

namespace ChessWithBot.Game;

public class Square
{
    public const int WIDTH = 75;
    public const int HEIGHT = 75;
    public Brush Color { get; init; }
    public Piece? Piece { get; set; }
    public (int, int) Coordinates { get; init; }

    public Square(Brush color, Piece? piece, (int, int) coordinates)
    {
        Color = color;
        Piece = piece;
        Coordinates = coordinates;
    }
}
