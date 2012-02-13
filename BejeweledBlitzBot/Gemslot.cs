using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BejeweledBlitzBot
{
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
    }

}

