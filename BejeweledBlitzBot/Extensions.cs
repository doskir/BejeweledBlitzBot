using System.Drawing;

namespace BejeweledBlitzBot
{
    public static class Extensions
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }
    }
}
