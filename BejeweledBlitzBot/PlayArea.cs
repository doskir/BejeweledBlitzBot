using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BejeweledBlitzBot
{
    internal class PlayArea
    {
        public GemSlot[,] GemSlots;
        private IGemClassifier _gemClassifier;

        public PlayArea(Image<Bgr, byte> screenshot, IGemClassifier gemClassifier)
        {
            _gemClassifier = gemClassifier;
            GemSlots = CreateGemslots(screenshot.Bitmap.Size);
            UpdateWithScreenshot(screenshot);
        }

        public PlayArea(GemSlot[,] gemSlots)
        {
            GemSlots = gemSlots;
        }

        /// <summary>
        /// Create the grid of gem slots
        /// </summary>
        /// <param name="playAreaResolution">The resolution of the game</param>
        /// <returns>The grid of gem slots in the [row,column] format</returns>
        public GemSlot[,] CreateGemslots(Size playAreaResolution)
        {
            //each gem slot is 40x40
            //row 1 column 0 top left is at 175,149
            //row 0 column 0 top left should be at 175,109
            GemSlot[,] gemSlots = new GemSlot[8,8];
            Point baseOffset = Point.Empty;
            Size gemAreaSize = Size.Empty;
            if (playAreaResolution.Width == 760 && playAreaResolution.Height == 596)
            {
                baseOffset = new Point(175, 109);
                gemAreaSize = new Size(40, 40);
            }
            else
            {
                throw new Exception("Wrong resolution, only 760x596 is accepted");
            }
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Point offset = new Point(column*gemAreaSize.Width, row*gemAreaSize.Height);
                    Point topLeft = new Point(baseOffset.X + offset.X, baseOffset.Y + offset.Y);
                    gemSlots[row, column] = new GemSlot(new Rectangle(topLeft, gemAreaSize));
                }

            }
            return gemSlots;
        }

        /// <summary>
        /// Update the current list of gems by detecting them from a screenshot
        /// </summary>
        /// <param name="screenshot">An image of the current playing area (without windowborders)</param>
        public void UpdateWithScreenshot(Image<Bgr, byte> screenshot)
        {
            ImageProcessing imageProcessing = new ImageProcessing();
            Image<Bgr, byte>[,] gemSlotImages = imageProcessing.ExtractGemSlots(screenshot, GemSlots);
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    GemSlots[row, column].Gem = _gemClassifier.ClassifyGem(gemSlotImages[row, column]);
                }
            }
        }

        /// <summary>
        /// Get the move that will result in the highest score after the specified number of moves
        /// </summary>
        /// <param name="moves">The number of moves to look ahead, 1 means the current move</param>
        /// <returns>The best move</returns>
        public Move GetBestMove(int moves)
        {
            if (moves < 1)
                throw new Exception();
            List<Move> possibleMoves = new List<Move>();
            Gem[,] gemArray = new Gem[8,8];
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    gemArray[row, column] = GemSlots[row, column].Gem;
                }
            }
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
                        Gem[,] newArray = (Gem[,]) gemArray.Clone();
                        SwapSlots(newArray, row, column, newRow, newColumn);
                        if (MatchExistsAt(newArray, newRow, newColumn))
                            possibleMoves.Add(new Move(row, column, newRow, newColumn));
                    }
                }
            }
            if (possibleMoves.Count == 0)
                return new Move(0, 0, 0, 0) {ValidMove = false};
            Random rand = new Random();
            return possibleMoves[rand.Next(possibleMoves.Count)];

        }

        bool MatchExistsAt(Gem[,] gemArray,int row, int column)
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
            Up,Down,Left,Right
        }

        void SwapSlots(Gem[,] gemArray,int fromRow,int fromColumn,int toRow,int toColumn)
        {
            Gem temp = gemArray[fromRow, fromColumn];
            gemArray[fromRow, fromColumn] = gemArray[toRow, toColumn];
            gemArray[toRow, toColumn] = temp;
        }
    }
}
