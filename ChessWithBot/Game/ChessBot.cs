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
        public Game Game { get; set; }
        public ChessBot(Game game)
        {
            Game = game;
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
                Game.UpdateBoard();
                Game.IsPlayerMove = true;
            }
        }

        private List<Move> GetAllPossibleMoves(Brush color)
        {
            var pieces = color == Brushes.White ? Game.Board.PlayerPeaces : Game.Board.BotPeaces;
            var moveSets = pieces.Select(x => x.PossibleMoves).Select(move => move).ToList();

            List<Move> moves = new List<Move>();

            foreach (var m in moveSets)
            {
                foreach (var movesInSet in m)
                {
                    moves.Add(movesInSet);
                }
            }

            return moves;
        }

        private Move? MiniMaxRoot(int depth)
        {
            var moves = GetAllPossibleMoves(Brushes.Brown);
            double bestMove = int.MinValue;
            Move? best = default;

            foreach (var move in moves)
            {
                var oldPosition = move.Piece.Position;
                Game.TryMakeMove(move);
                double value = MiniMax(depth - 1, double.MinValue, double.MaxValue, false);
                Game.UndoMove(oldPosition, move);
                if (move.IsShortCastling || move.IsLongCastling)
                {
                    Game.CancelCastling(move);
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
                return -Game.Board.EvaluateMaterial();
            }
            if (isMaximising)
            {
                var moves = GetAllPossibleMoves(Brushes.White);
                double bestMove = int.MinValue;
                foreach (var move in moves)
                {
                    var piece = move.Piece;
                    var oldPosition = piece.Position;
                    Game.TryMakeMove(move);
                    bestMove = Math.Max(bestMove, MiniMax(depth - 1, alpha, beta, false));
                    Game.UndoMove(oldPosition, move);
                    if (move.IsShortCastling || move.IsLongCastling)
                    {
                        Game.CancelCastling(move);
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
                foreach (var move in GetAllPossibleMoves(Brushes.Brown))
                {
                    var piece = move.Piece;
                    var oldPosition = piece.Position;
                    Game.TryMakeMove(move);
                    bestMove = Math.Min(bestMove, MiniMax(depth - 1, alpha, beta, true));
                    Game.UndoMove(oldPosition, move);
                    if (move.IsShortCastling || move.IsLongCastling)
                    {
                        Game.CancelCastling(move);
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
