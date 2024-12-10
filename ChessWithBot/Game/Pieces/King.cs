using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChessWithBot.Game.Pieces;

public class King : Piece, IMoveDependent
{

    public King(Brush color, (int, int) position, Board board) : base(color, PieceType.King, position, board)
    {
        MovesCount = 0;
        
    }

    public int MovesCount { get; set; }
    protected override double[,] PiecePositionMatrix
    {
        get
        {
            return new double[,] {
                { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
                { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
                { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
                { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
                { -2.0, -3.0, -3.0, -4.0, -4.0, -3.0, -3.0, -2.0 },
                { -1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0 },
                {  2.0,  2.0,  0.0,  0.0,  0.0,  0.0,  2.0,  2.0 },
                {  2.0,  3.0,  1.0,  0.0,  0.0,  1.0,  3.0,  2.0 }
            };
        }
    }

    public override int Weight => 900;

    public override bool Move(Move move, HashSet<Move> possibleMoves)
    {
        if (possibleMoves.Any(m => m.Coordinates == move.Coordinates && (m.IsShortCastling || m.IsLongCastling)))
        {
            var castlingMove = possibleMoves.Where(move => move.IsShortCastling || move.IsLongCastling
                && move.Coordinates == move.Coordinates);
            if (castlingMove.Any())
            {
                Move castling = castlingMove.First();
                // Длинная рокировка
                if (castling.IsLongCastling)
                {
                    int line = Color == Brushes.White ? Board.BOARD_HEIGHT - 1 : 0;
                    var oldPosition = Position;
                    Position = move.Coordinates;
                    Piece leftRook = Board.Squares[line, 0].Piece!;
                    Move rookMove = new Move(leftRook, (line, 3));
                    leftRook.PossibleMoves.Add(rookMove);
                    leftRook.Move(rookMove, leftRook.PossibleMoves);
                    Board.Squares[oldPosition.X, oldPosition.Y].Piece = null;
                    Board.Squares[Position.X, Position.Y].Piece = this;
                }
                // короткая
                else
                {
                    int line = Color == Brushes.White ? Board.BOARD_HEIGHT - 1 : 0;
                    var oldPosition = Position;
                    Position = move.Coordinates;
                    Piece rightRook = Board.Squares[line, 7].Piece!;
                    Move rookMove = new Move(rightRook, (line, 5));
                    rightRook.PossibleMoves.Add(rookMove);
                    rightRook.Move(rookMove, rightRook.PossibleMoves);
                    Board.Squares[oldPosition.X, oldPosition.Y].Piece = null;
                    Board.Squares[Position.X, Position.Y].Piece = this;
                }
                MovesCount++;
                return true;
            }
        }
        return base.Move(move, possibleMoves);
    }

    public override void ValidateMoves(HashSet<Move> moves)
    {
        bool isPlayer = Color == Brushes.White;
        List<Piece> enemies = isPlayer ? Board.BotPeaces : Board.PlayerPeaces;
        Square currentSquare = Board.Squares[Position.X, Position.Y];
        foreach (var move in moves)
        {
            Piece? newSquarePiece = Board.Squares[move.Coordinates.X, move.Coordinates.Y].Piece;
            Move(move, moves);
            foreach (var p in enemies)
            {
                var enemyMoves = p.GeneratePossibleMoves();
                if (enemyMoves.Any(m => m.Coordinates == move.Coordinates))
                {
                    moves.Remove(move);
                    break;
                }
            }
            UndoMove(currentSquare.Coordinates, move, newSquarePiece);
            if (move.IsShortCastling)
            {
                int line = isPlayer ? Board.BOARD_WIDTH - 1 : 0;
                var rook = Board.Squares[line, Board.BOARD_WIDTH - 3].Piece!;

                rook.UndoMove((line, Board.BOARD_WIDTH - 1), new Move(rook, (line, Board.BOARD_WIDTH - 3)), null);
            }
            else if (move.IsLongCastling)
            {
                int line = isPlayer ? Board.BOARD_WIDTH - 1 : 0;
                var rook = Board.Squares[line, 3].Piece!;
                rook.UndoMove((line, 0), new Move(rook, (line, 3)), null);
            }
        }
    }

    public override HashSet<Move> GeneratePossibleMoves()
    {
        var moves = new HashSet<Move>();
        BishopTypeMovements(moves, true);
        RookTypeMovement(moves, true);
        if (MovesCount == 0)
        {
            int line = Color == Brushes.White ? Board.BOARD_HEIGHT - 1 : 0;
            Piece? p1 = Board.Squares[line, 0].Piece;
            Piece? p2 = Board.Squares[line, Board.BOARD_WIDTH - 1].Piece;
            if (p1 != null && p1 is Rook leftRook && leftRook.MovesCount == 0)
            {
                bool flag = true;
                for (int i = 1; i < 4; i++)
                {
                    if (Board.Squares[line, i].Piece != null)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    moves.Add(new Move(this, (line, 2))
                    {
                        IsLongCastling = true
                    });
                }
            }

            if (p2 != null && p2 is Rook rightRook && rightRook.MovesCount == 0)
            {
                bool flag = true;
                for (int i = 5; i < 7; i++)
                {
                    if (Board.Squares[line, i].Piece != null)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    moves.Add(new Move(this, (line, 6))
                    {
                        IsShortCastling = true
                    });
                }
            }
        }
        return moves;
    }
}
