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
        public Gem[,] ToGemArray()
        {
            Gem[,] gemArray = new Gem[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    gemArray[row, column] = GemSlots[row, column].Gem;
                }
            }
            return gemArray;
        }
    }
}
