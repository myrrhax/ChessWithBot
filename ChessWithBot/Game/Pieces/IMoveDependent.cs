using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWithBot.Game.Pieces;

public interface IMoveDependent
{
    public int MovesCount { get; set; }
}
