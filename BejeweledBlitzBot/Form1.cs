using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Form = System.Windows.Forms.Form;

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
// ReSharper disable PossibleNullReferenceException
// if this is 0 we can't do anything anyway
            webBrowser1.Document.Window.ScrollTo(targetScrollPosition);
// ReSharper restore PossibleNullReferenceException
            if (_gameInterfacer == null)
            {
                _gameInterfacer = new GameInterfacer(webBrowser1);
                Debug.WriteLine(Handle);
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
