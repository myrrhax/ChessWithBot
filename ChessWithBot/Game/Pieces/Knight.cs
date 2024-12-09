using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChessWithBot.Game.Pieces;

public class Knight : Piece
{
    public Knight(Brush color, (int, int) position, Board board) : base(color, PieceType.Knight, position, board)
    {
    }

    protected override double[,] PiecePositionMatrix => new double[,] {
        {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 },
        {-4.0, -2.0,  0.0,  0.0,  0.0,  0.0, -2.0, -4.0 },
        {-3.0,  0.0,  1.0,  1.5,  1.5,  1.0,  0.0, -3.0 },
        {-3.0,  0.5,  1.5,  2.0,  2.0,  1.5,  0.5, -3.0 },
        {-3.0,  0.0,  1.5,  2.0,  2.0,  1.5,  0.0, -3.0 },
        {-3.0,  0.5,  1.0,  1.5,  1.5,  1.0,  0.5, -3.0 },
        {-4.0, -2.0,  0.0,  0.5,  0.5,  0.0, -2.0, -4.0 },
        {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 }
    };

    public override HashSet<Move> GeneratePossibleMoves()
    {
        var res = new HashSet<Move>();

        var squares = Board.Squares;
        Move m;

        for (int i = 0; i < 2; i++)
        {
            int vert = i % 2 == 0 ? 1 : -1;
            int hor;
            for (int j = 0; j < 2; j++)
            {
                hor = j % 2 == 0 ? 1 : -1;
                m = new Move(this, (Position.X + vert * 2, Position.Y + hor));
                if (checkMapCollisions(m.Coordinates))
                {
                    Square sq = squares[m.Coordinates.X, m.Coordinates.Y];
                    if (sq.Piece == null || sq.Piece.Color != Color)
                    {
                        if (sq.Piece?.Color != Color) m.TakedPiece = sq.Piece;
                        res.Add(m);
                    }
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int vert = i % 2 == 0 ? 1 : -1;
            int hor;
            for (int j = 0; j < 2; j++)
            {
                hor = j % 2 == 0 ? 1 : -1;
                m = new Move(this, (Position.X + hor, Position.Y + vert * 2));
                if (checkMapCollisions(m.Coordinates))
                {
                    Square sq = squares[m.Coordinates.X, m.Coordinates.Y];
                    if (sq.Piece == null || sq.Piece.Color != Color)
                    {
                        if (sq.Piece?.Color != Color) m.TakedPiece = sq.Piece;
                        res.Add(m);
                    }
                }
            }
        }

        return res;
    }

    private bool checkMapCollisions((int x, int y) coords)
    {
        return coords.x >= 0 && coords.x < Board.BOARD_WIDTH && coords.y >= 0 && coords.y < Board.BOARD_HEIGHT;
    }

}
