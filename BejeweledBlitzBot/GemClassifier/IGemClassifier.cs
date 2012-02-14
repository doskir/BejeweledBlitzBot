using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BejeweledBlitzBot.GemClassifier
{
    interface IGemClassifier
    {
        Gem ClassifyGem(Image<Bgr, Byte> gemSlotImage);

    }
}
