using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BejeweledBlitzBot
{
    interface IGemClassifier
    {
        Gem ClassifyGem(Image<Bgr, Byte> gemSlotImage);

    }
}
