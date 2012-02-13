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
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Form = System.Windows.Forms.Form;
using Timer = System.Timers.Timer;

namespace BejeweledBlitzBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        private GameInterfacer _gameInterfacer;
        private Logic _logic;
        private void button1_Click(object sender, EventArgs e)
        {
            //scroll to the flash element of the game
            Point targetScrollPosition = new Point(7,38 + 156);
            webBrowser1.Document.Window.ScrollTo(targetScrollPosition);
            if (_gameInterfacer == null)
            {
                _gameInterfacer = new GameInterfacer(webBrowser1);
                Debug.WriteLine(this.Handle);
            }
            _logic = new Logic(_gameInterfacer);
            _logic.StartBot();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            _logic.StopBot();
        }
    }
}
