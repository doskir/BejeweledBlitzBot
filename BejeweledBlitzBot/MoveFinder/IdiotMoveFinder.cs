using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot
{
    class IdiotMoveFinder : IMoveFinder
    {
        public Move GetBestMove(Gem[,] gems, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                throw new Exception();
            List<Move> possibleMoves = new List<Move>();
            //for now simply return the first match (from the top left) we find
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    List<Direction> validDirections = new List<Direction>();
                    if (row > 0)
                        validDirections.Add(Direction.Up);
                    if (row < 7)
                        validDirections.Add(Direction.Down);
                    if (column > 0)
                        validDirections.Add(Direction.Left);
                    if (column < 7)
                        validDirections.Add(Direction.Right);
                    foreach (Direction direction in validDirections)
                    {
                        int newRow = row;
                        int newColumn = column;
                        switch (direction)
                        {
                            case Direction.Up:
                                newRow = row - 1;
                                break;
                            case Direction.Down:
                                newRow = row + 1;
                                break;
                            case Direction.Left:
                                newColumn = column - 1;
                                break;
                            case Direction.Right:
                                newColumn = column + 1;
                                break;
                        }
                        Gem[,] newArray = (Gem[,])gems.Clone();
                        SwapSlots(newArray, row, column, newRow, newColumn);
                        if (MatchExistsAt(newArray, newRow, newColumn))
                            possibleMoves.Add(new Move(row, column, newRow, newColumn));
                    }
                }
            }
            if (possibleMoves.Count == 0)
                return new Move(0, 0, 0, 0) { ValidMove = false };
            Random rand = new Random();
            return possibleMoves[rand.Next(possibleMoves.Count)];
        }
        bool MatchExistsAt(Gem[,] gemArray, int row, int column)
        {
            GemColor myColor = gemArray[row, column].Color;
            //check for a horizontal match with the current gem at the left
            if (column < 6)
                if (gemArray[row, column + 1].Color == myColor
                    && gemArray[row, column + 2].Color == myColor)
                    return true;
            //check for a horizontal match with the current gem at the center
            if (column > 0 && column < 7)
                if (gemArray[row, column - 1].Color == myColor
                    && gemArray[row, column + 1].Color == myColor)
                    return true;
            //check for a horizontal mathc with the current gem at the right
            if (column > 1)
                if (gemArray[row, column - 1].Color == myColor
                    && gemArray[row, column - 2].Color == myColor)
                    return true;
            //check for a vertical match with the current gem at the top
            if (row < 6)
                if (gemArray[row + 1, column].Color == myColor
                    && gemArray[row + 2, column].Color == myColor)
                    return true;
            //check for a vertical match with the current gem at the center
            if (row > 0 && row < 7)
                if (gemArray[row - 1, column].Color == myColor
                    && gemArray[row + 1, column].Color == myColor)
                    return true;
            //check for a vertical match with the current gem at the bottom
            if (row > 1)
                if (gemArray[row - 1, column].Color == myColor
                    && gemArray[row - 2, column].Color == myColor)
                    return true;

            return false;
        }
        enum Direction
        {
            Up, Down, Left, Right
        }

        void SwapSlots(Gem[,] gemArray, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            Gem temp = gemArray[fromRow, fromColumn];
            gemArray[fromRow, fromColumn] = gemArray[toRow, toColumn];
            gemArray[toRow, toColumn] = temp;
        }
    }
}
