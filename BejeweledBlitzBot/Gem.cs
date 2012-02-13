using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BejeweledBlitzBot
{
    public enum GemColor
    {
        Unknown,
        Any,
        Blue,
        Green,
        Orange,
        Red,
        Violet,
        White,
        Yellow

    };

    public enum GemType
    {
        Unknown,Normal,Flame,Star,Hypercube,Supernova
    }

    class Gem
    {
        public GemColor Color;
        public GemType Type;
        public Gem(GemColor color,GemType type)
        {
            Color = color;
            Type = type;
        }

    }
}
