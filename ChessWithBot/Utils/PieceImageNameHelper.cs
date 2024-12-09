using ChessWithBot.Game.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessWithBot.Utils;

public static class PieceImageNameHelper
{
    public const string DIRECTORY_NAME = "C:\\Users\\keffi\\source\\repos\\ChessWithBot\\ChessWithBot\\Images";
    public static string GetPieceImagePath(PieceType pt, Brush color)
    {
        string colorName = color == Brushes.White ? "white" : "black";
        return $"{DIRECTORY_NAME}\\{colorName}_{pt.ToString().ToLower()}.png";
    }
}
