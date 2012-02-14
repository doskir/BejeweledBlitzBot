using System;
using System.Collections.Generic;

namespace BejeweledBlitzBot.MoveFinder
{
    class IdiotMoveFinder : IMoveFinder
    {
        public Move GetBestMove(Gem[,] gems, int movesToLookAhead, List<Position> lockedOutPositions)
        {
            if (movesToLookAhead < 1)
                throw new Exception();
            List<Move> possibleMoves = new List<Move>();
            //for now simply return the first match (from the top left) we find
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Position position = new Position(row, column);
                    if (lockedOutPositions.Contains(position))
                        continue;
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
                        Position newPosition = new Position(newRow, newColumn);
                        if (lockedOutPositions.Contains(newPosition))
                            continue;
                        Gem[,] newArray = (Gem[,])gems.Clone();
                        SwapSlots(newArray, row, column, newRow, newColumn);
                        List<Position> involvedGems = MatchedGemsAt(newArray, new Position(newRow, newColumn));
                        if (involvedGems.Count >= 2)
                            possibleMoves.Add(new Move(new Position(row, column), new Position(newRow, newColumn), involvedGems) { GuaranteedScore = involvedGems.Count * 100 });
                    }
                }
            }
            if (possibleMoves.Count == 0)
                return new Move(new Position(0, 0), new Position(0, 0)) {ValidMove = false};
            Random rand = new Random();
            return possibleMoves[rand.Next(possibleMoves.Count)];
        }
        List<Position> MatchedGemsAt(Gem[,] gemArray, Position origin)
        {
            GemColor myColor = gemArray[origin.Row, origin.Column].Color;
            List<Position> matchingGemsLeft = new List<Position>();
            for (int column = origin.Column - 1; column >= 0; column--)
            {
                if (gemArray[origin.Row, column].Color == myColor)
                    matchingGemsLeft.Add(new Position(origin.Row, column));
                else
                    break;
            }
            List<Position> matchingGemsRight = new List<Position>();
            for (int column = origin.Column + 1; column < 8; column++)
            {
                if (gemArray[origin.Row, column].Color == myColor)
                    matchingGemsRight.Add(new Position(origin.Row, column));
                else
                    break;
            }

            List<Position> matchingGemsAbove = new List<Position>();
            for (int row = origin.Row - 1; row >= 0; row--)
            {
                if (gemArray[row, origin.Column].Color == myColor)
                    matchingGemsAbove.Add(new Position(row, origin.Column));
                else
                    break;
            }
            List<Position> matchingGemsBelow = new List<Position>();
            for (int row = origin.Row + 1; row < 8; row++)
            {
                if (gemArray[row, origin.Column].Color == myColor)
                    matchingGemsBelow.Add(new Position(row, origin.Column));
                else
                    break;
            }
            List<Position> horizontalMatch = new List<Position>(matchingGemsLeft);
            horizontalMatch.AddRange(matchingGemsRight);
            List<Position> verticalMatch = new List<Position>(matchingGemsAbove);
            verticalMatch.AddRange(matchingGemsBelow);

            if (horizontalMatch.Count < 2)
                horizontalMatch.RemoveRange(0, horizontalMatch.Count);
            if (verticalMatch.Count < 2)
                verticalMatch.RemoveRange(0, verticalMatch.Count);

            if (horizontalMatch.Count >= 3)
            {
                //fire gem will be created
            }
            else if (horizontalMatch.Count >= 2 && verticalMatch.Count >= 2)
            {
                //star gem will be created
            }

            List<Position> involvedGems = new List<Position>(horizontalMatch);
            involvedGems.AddRange(verticalMatch);
            return involvedGems;
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
