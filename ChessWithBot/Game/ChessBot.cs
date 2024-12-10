using ChessWithBot.Game.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChessWithBot.Game
{
    public class ChessBot
    {
        int movesCount = 0;
        public Game Game { get; set; }
        Random random = new Random();

        public ChessBot(Game game)
        {
            Game = game;
        }

        private int GetRandomOffset()
        {
            return random.Next(1, 10); // Случайное число от -0.05 до 0.05
        }

        public void MakeMove()
        {

            if (!Game.IsPlayerMove)
            {
                var bestMove = MiniMaxRoot(3);
                if (bestMove == null)
                {
                    Game.NoPossibleMoves(Brushes.Brown);
                }
                
                if (bestMove.Value.IsAttacking)
                {
                    Game.TakePieceByBot(bestMove!.Value.Coordinates);
                }   
                Game.TryMakeMove(bestMove!.Value);
                MessageBox.Show("Calculated: " + movesCount);
                Game.UpdateBoard();
                movesCount = 0;
                Game.IsPlayerMove = true;
            }
        }

        private List<Move> GetOrderedMoves(bool isMaximising)
        {
            var moves = Game.GetAllPossibleMoves(isMaximising ? Brushes.White : Brushes.Brown);
            return moves.OrderByDescending(move =>
                move.IsQueening ? 90 : 0)
                .ThenByDescending(move =>
                move.IsAttacking ? 10 + move.TakedPiece!.Weight : 0)// Взятие приоритетно
                .ToList();
        }

        private Move? IterativeDeepening(int maxDepth)
        {
            Move? bestMove = null;
            for (int depth = 1; depth <= maxDepth; depth++)
            {
                bestMove = MiniMaxRoot(depth);
            }
            return bestMove;
        }

        private Move? MiniMaxRoot(int depth)
        {
            var moves = GetOrderedMoves(false);
            double bestMove = int.MinValue;
            Move? best = default;

            foreach (var move in moves)
            {
                movesCount++;
                var oldPosition = move.Piece.Position;
                Game.TryMakeMove(move);
                double value = MiniMax(depth - 1, double.MinValue, double.MaxValue, false);
                Game.UndoMove(oldPosition, move);
                if (move.IsShortCastling || move.IsLongCastling)
                {
                    if (move.IsShortCastling || move.IsLongCastling)
                    {
                        int line = move.Piece.Color == Brushes.White ? Board.BOARD_HEIGHT - 1 : 0;
                        int y = move.IsShortCastling ? Board.BOARD_WIDTH - 3 : 3;
                        var rook = Game.Board.Squares[line, y].Piece! as Rook;
                        rook!.CancelCastling(move.IsShortCastling);
                    }
                }
                if (value >= bestMove)
                {
                    bestMove = value;
                    best = move;
                }
            }

            return best;
        }

        private double MiniMax(int depth, double alpha, double beta, bool isMaximising)
        {
            if (depth == 0)
            {
                return -Game.Board.EvaluateMaterial() + GetRandomOffset();
            }
            if (isMaximising)
            {
                var moves = GetOrderedMoves(false);
                double bestMove = int.MinValue;
                foreach (var move in moves)
                {
                    movesCount++;
                    var piece = move.Piece;
                    var oldPosition = piece.Position;
                    Game.TryMakeMove(move);
                    bestMove = Math.Max(bestMove, MiniMax(depth - 1, alpha, beta, false));
                    Game.UndoMove(oldPosition, move);
                    if (move.IsShortCastling || move.IsLongCastling)
                    {
                        int line = move.Piece.Color == Brushes.White ? Board.BOARD_HEIGHT - 1 : 0;
                        int y = move.IsShortCastling ? Board.BOARD_WIDTH - 3 : 3;
                        var rook = Game.Board.Squares[line, y].Piece! as Rook;
                        rook!.CancelCastling(move.IsShortCastling);
                    }
                    alpha = Math.Max(alpha, bestMove);
                    if (beta <= alpha)
                    {
                        return bestMove;
                    }
                }
                return bestMove;
            }
            else
            {
                double bestMove = int.MaxValue;
                foreach (var move in GetOrderedMoves(true))
                {
                    movesCount++;
                    var piece = move.Piece;
                    var oldPosition = piece.Position;
                    Game.TryMakeMove(move);
                    bestMove = Math.Min(bestMove, MiniMax(depth - 1, alpha, beta, true));
                    Game.UndoMove(oldPosition, move);
                    if (move.IsShortCastling || move.IsLongCastling)
                    {
                        int line = move.Piece.Color == Brushes.White ? Board.BOARD_HEIGHT - 1 : 0;
                        int y = move.IsShortCastling ? Board.BOARD_WIDTH - 3 : 3;
                        var rook = Game.Board.Squares[line, y].Piece! as Rook;
                        rook!.CancelCastling(move.IsShortCastling);
                    }
                    beta = Math.Min(beta, bestMove);
                    if (beta <= alpha)
                    {
                        return bestMove;
                    }
                }
                return bestMove;
            }
            
        }
    }
}
