using System;
using System.Diagnostics;
using System.Drawing;

namespace BejeweledBlitzBot
{
    [DebuggerDisplay("{DebuggerDisplay()}")]
    class GemSlot
    {
        public Rectangle Rectangle;
        public Gem Gem;
        public DateTime LockedUntil = DateTime.MinValue;
        public GemSlot(Rectangle rectangle) : this(rectangle, new Gem(GemColor.Unknown, GemType.Unknown))
        {
            
        }

        public GemSlot(Rectangle rectangle,Gem gem)
        {
            Rectangle = rectangle;
            Gem = gem;
        }
// ReSharper disable UnusedMember.Local
        private string DebuggerDisplay()
// ReSharper restore UnusedMember.Local
        {
            return Enum.GetName(typeof (GemColor), Gem.Color);
        }
    }

}

