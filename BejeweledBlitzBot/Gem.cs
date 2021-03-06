﻿namespace BejeweledBlitzBot
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
