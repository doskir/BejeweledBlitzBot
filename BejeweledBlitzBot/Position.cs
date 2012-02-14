using System;

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
            return Row == other.Row && Column == other.Column;
        }
    }
}
