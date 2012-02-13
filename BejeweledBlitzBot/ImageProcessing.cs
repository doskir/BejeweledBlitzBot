#define SAVEIMAGES
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
namespace BejeweledBlitzBot
{
    class ImageProcessing
    {
        /// <summary>
        /// Extracts each gems rectangle in the image
        /// </summary>
        /// <param name="screenshot">An image of the current playing area (without windowborders)</param>
        /// <returns>A two-dimensional array of images in the [row,column] format</returns>
        public Image<Bgr, byte>[,] ExtractGemSlots(Image<Bgr, byte> screenshot, GemSlot[,] gemSlots)
        {
#if SAVEIMAGES
            string basePath = "abc\\";
            foreach(GemColor color in Enum.GetValues(typeof(GemColor)))
            {
                if (!System.IO.Directory.Exists(basePath + color))
                    System.IO.Directory.CreateDirectory(basePath + color);
            }

#endif
            Image<Bgr, byte>[,] gemSlotImages = new Image<Bgr, byte>[8,8];
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Rectangle gemSlotRectangle = gemSlots[row, column].Rectangle;
                    gemSlotImages[row, column] = screenshot.Copy(gemSlotRectangle);
#if SAVEIMAGES
                    Random rand = new Random();
                    Gem prediction = (new SimpleGemClassifier().ClassifyGem(gemSlotImages[row, column]));
                    gemSlotImages[row, column].Save("abc\\" + prediction.Color.ToString() + "\\" + rand.Next() + "." + row + "." + column + ".png");
#endif
                }
            }
            return gemSlotImages;
        }
    }
}
