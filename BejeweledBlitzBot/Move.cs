using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot
{
    internal class Move
    {
        public Position From;
        public Position To;
        public bool ValidMove = true;
        public int GuaranteedScore;
        public List<Position> UsedPositions = new List<Position>(); 
        public Move(Position from, Position to,List<Position> usedPositions)
        {
            From = from;
            To = to;
            UsedPositions = usedPositions;
        }

        public Move(Position from, Position to) : this(from, to, null)
        {
            
        }
    }
}