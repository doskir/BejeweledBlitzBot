using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot.MoveFinder
{
    class ChainFinder : IMoveFinder
    {
        public Move GetBestMove(Gem[,] gems, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                throw new Exception();
            List<Move> possibleMoves = new List<Move>();
            return null;
        }
    }
}
