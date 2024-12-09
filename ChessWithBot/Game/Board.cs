using ChessWithBot.Game.Pieces;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChessWithBot.Game;

public class Board
{
    public const int BOARD_WIDTH = 8;
    public const int BOARD_HEIGHT = 8;
    private readonly PieceType[] pieces = { PieceType.Rook, PieceType.Knight, PieceType.Bishop, 
        PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
    public Square[,] Squares { get; init; }
    public List<Piece> BotPeaces { get; init; } = new List<Piece>(15);
    public List<Piece> PlayerPeaces { get; init; } = new List<Piece>(15);
    public King PlayerKing { get; private set; }
    public King BotKing { get; private set; }

    public Board()
    {
        Squares = new Square[BOARD_WIDTH, BOARD_HEIGHT];
        InitializeBoard();
    }

    public void UpdatePiecesPossibleMoves()
    {
        foreach (var p in PlayerPeaces)
        {
            var moves = p.GeneratePossibleMoves();
            p.ValidateMoves(moves);
            p.PossibleMoves = moves;
        }
        foreach (var p in BotPeaces)
        {
            var moves = p.GeneratePossibleMoves();
            p.ValidateMoves(moves);
            p.PossibleMoves = moves;
        }

        var playerKingMoves = PlayerKing.GeneratePossibleMoves();
        PlayerKing.ValidateMoves(playerKingMoves);
        PlayerKing.PossibleMoves = playerKingMoves;
        var botKingMoves = BotKing.GeneratePossibleMoves();
        BotKing.ValidateMoves(botKingMoves);
        BotKing.PossibleMoves = botKingMoves;
    }

    public double EvaluateMaterial()
    {
        double score = 0;
        var piecesInGame = BotPeaces.Union(PlayerPeaces);

        foreach (var piece in piecesInGame)
        {
            int modifier = piece.Color == Brushes.White ? 1 : -1;
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    score += modifier * 10;
                    break;
                case PieceType.Knight:
                    score += modifier * 30; 
                    break;
                case PieceType.Bishop:
                    score += modifier * 30;
                    break;
                case PieceType.Rook:
                    score += modifier * 50;
                    break;
                case PieceType.Queen:
                    score += modifier * 90;
                    break;
                case PieceType.King:
                    score += modifier * 900;
                    break;
            }
            score += piece.PositionMatrix[piece.Position.X, piece.Position.Y];
        }

        return score;
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < BOARD_HEIGHT; i++)
        {
            for (int j = 0; j < BOARD_WIDTH; j++)
            {
                Brush c = i % 2 == j % 2 ? Brushes.White : Brushes.Brown;
                Squares[i, j] = new Square(c, null, (i, j));
                if (i == 0 || i == 7)
                {
                    Brush pieceColor = i == 0 ? Brushes.Brown : Brushes.White;
                    Squares[i, j].Piece = PieceFactory.GetPiece(pieces[j], (i, j), pieceColor, this);
                    if (pieces[j] == PieceType.King)
                    {
                        if (i == 0) BotKing = (Squares[i, j].Piece as King)!;
                        else PlayerKing = (Squares[i, j].Piece as King)!;
                    }
                    else
                    {
                        if (i == 0) BotPeaces.Add(Squares[i, j].Piece!);
                        else PlayerPeaces.Add(Squares[i, j].Piece!);
                    }
                }
                else if (i == 1 || i == 6)
                {
                    Brush pieceColor = i == 1 ? Brushes.Brown : Brushes.White;
                    Squares[i, j].Piece = PieceFactory.GetPiece(PieceType.Pawn, (i, j), pieceColor, this);
                    if (i == 1) BotPeaces.Add(Squares[i, j].Piece!);
                    else PlayerPeaces.Add(Squares[i, j].Piece!);
                }
            }
        }
        foreach (var piece in PlayerPeaces)
        {
            piece.PossibleMoves = piece.GeneratePossibleMoves();
        }
        PlayerKing.PossibleMoves = PlayerKing.GeneratePossibleMoves();
        BotKing.PossibleMoves = BotKing.GeneratePossibleMoves();
    }
}
