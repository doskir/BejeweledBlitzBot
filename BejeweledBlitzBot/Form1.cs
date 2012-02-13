using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Form = System.Windows.Forms.Form;

namespace BejeweledBlitzBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private GameInterfacer flashAutomation;
        private void button1_Click(object sender, EventArgs e)
        {
            //scroll to the flash element of the game
            Point targetScrollPosition = new Point(7,38 + 156);
            webBrowser1.Document.Window.ScrollTo(targetScrollPosition);
            flashAutomation = new GameInterfacer(webBrowser1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var image = flashAutomation.ScreenShot();
            image.Save("!test.png");

        }
        private void button3_Click(object sender, EventArgs e)
        {
            flashAutomation.Click(276, 291);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PlayArea playArea = new PlayArea(new Image<Bgr, byte>("!gems.png"), new SimpleGemClassifier());
        }
    }
}
