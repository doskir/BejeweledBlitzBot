using System.Collections.Generic;

namespace BejeweledBlitzBot.MoveFinder
{
    interface IMoveFinder
    {
        Move GetBestMove(Gem[,] gems, int movesToLookAhead,List<Position> lockedOutPositions);
    }
}
