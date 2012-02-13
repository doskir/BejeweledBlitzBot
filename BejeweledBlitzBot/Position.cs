using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot
{
    internal class Position : IEquatable<Position>
    {
        public int Row;
        public int Column;
        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool Equals(Position other)
        {
            return this.Row == other.Row && this.Column == other.Column;
        }
    }
}
