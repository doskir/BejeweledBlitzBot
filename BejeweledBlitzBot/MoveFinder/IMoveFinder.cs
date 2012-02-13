using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot
{
    interface IMoveFinder
    {
        Move GetBestMove(Gem[,] gems, int movesToLookAhead,List<Position> lockedOutPositions);
    }
}
