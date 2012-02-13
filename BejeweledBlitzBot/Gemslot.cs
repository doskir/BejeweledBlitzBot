using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot
{
    [DebuggerDisplay("{DebuggerDisplay()}")]
    class GemSlot
    {
        public Rectangle Rectangle;
        public Gem Gem;
        public GemSlot(Rectangle rectangle) : this(rectangle, new Gem(GemColor.Unknown, GemType.Unknown))
        {
            
        }

        public GemSlot(Rectangle rectangle,Gem gem)
        {
            Rectangle = rectangle;
            Gem = gem;
        }
        private string DebuggerDisplay()
        {
            return Enum.GetName(typeof (GemColor), Gem.Color);
        }
    }

}

