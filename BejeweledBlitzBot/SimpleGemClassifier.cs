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
    class SimpleGemClassifier : IGemClassifier
    {
        private static readonly Bgr BlueGemBgr = new Bgr(50.718125, 26.8925,3.520625);
        private static readonly Bgr GreenGemBgr = new Bgr(18.84125, 53.569375, 10.765);
        private static readonly Bgr OrangeGemBgr = new Bgr(13.935625, 35.88375, 59.6225);
        private static readonly Bgr RedGemBgr = new Bgr(15.5175, 7.82, 60.71375);

        private static readonly Bgr VioletGemBgr = new Bgr(44.038203125, 9.5991964,45.0125);
        private static readonly Bgr WhiteGemBgr = new Bgr(54.6975, 54.6975, 54.6975);
        private static readonly Bgr YellowGemBgr = new Bgr(6.726875, 49.238125,56.595625);



        public Gem ClassifyGem(Image<Bgr, byte> gemSlotImage)
        {
            if(gemSlotImage.Bitmap.Width != 40 || gemSlotImage.Bitmap.Height != 40)
                throw new Exception("only 40x40 gem slots are accepted");
            Image<Bgr, byte> centerPart = gemSlotImage.GetSubRect(new Rectangle(10, 10, 20, 20));

            Bgr sum = centerPart.GetSum();
            int pixels = gemSlotImage.Bitmap.Width*gemSlotImage.Bitmap.Height;
            Bgr averageBgr = new Bgr(sum.Blue/pixels, sum.Green/pixels, sum.Red/pixels);


            double smallestDistance = double.MaxValue;
            GemColor color = GemColor.Unknown;


            double blueDistance = Distance(averageBgr, BlueGemBgr);
            if(blueDistance < smallestDistance)
            {
                smallestDistance = blueDistance;
                color = GemColor.Blue;
            }
            double greenDistance = Distance(averageBgr, GreenGemBgr);
            if(greenDistance < smallestDistance)
            {
                smallestDistance = greenDistance;
                color = GemColor.Green;
            }
            double orangeDistance = Distance(averageBgr, OrangeGemBgr);
            if(orangeDistance < smallestDistance)
            {
                smallestDistance = orangeDistance;
                color = GemColor.Orange;
            }
            double redDistance = Distance(averageBgr, RedGemBgr); 
            if(redDistance < smallestDistance)
            {
                smallestDistance = redDistance;
                color = GemColor.Red;
            }
            double violetDistance = Distance(averageBgr, VioletGemBgr);
            if(violetDistance < smallestDistance)
            {
                smallestDistance = violetDistance;
                color = GemColor.Violet;
            }

            double whiteDistance = Distance(averageBgr, WhiteGemBgr);
            if(whiteDistance < smallestDistance)
            {
                smallestDistance = whiteDistance;
                color = GemColor.White;
            }

            double yellowDistance = Distance(averageBgr, YellowGemBgr);
            if(yellowDistance < smallestDistance)
            {
                smallestDistance = yellowDistance;
                color = GemColor.Yellow;
            }

            Debug.WriteLine("{0}: {1}", color, averageBgr);
            return new Gem(color, GemType.Normal);
        }
        /// <summary>
        /// A pretty standard euclidean distance implementation to find the distance between two colors
        /// </summary>
        /// <param name="first">Some color</param>
        /// <param name="second">Some other color</param>
        /// <returns>The distance between the 2 colors</returns>
        private double Distance(Bgr first,Bgr second)
        {
            Bgr diff = new Bgr(first.Blue - second.Blue, first.Green - second.Green, first.Red - second.Red);
            Bgr squared = new Bgr(diff.Blue*diff.Blue, diff.Green*diff.Green, diff.Red*diff.Red);
            double sumOfSquares = squared.Blue + squared.Green + squared.Red;
            return Math.Sqrt(sumOfSquares);
        }
    }
}
