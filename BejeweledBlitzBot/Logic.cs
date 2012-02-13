using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace BejeweledBlitzBot
{
    class Logic
    {
        private GameInterfacer _gameInterfacer;
        private ImageProcessing _imageProcessing = new ImageProcessing();
        private IGemClassifier _gemClassifier = new SimpleGemClassifier();
        private PlayArea _currentPlayArea;
        private Thread _botThread;
        private bool _running;
        public Logic(GameInterfacer gameInterfacer)
        {
            _gameInterfacer = gameInterfacer;
            
        }
        public void StartBot()
        {
            //click the play now button
            _gameInterfacer.Click(268, 408);
            Thread.Sleep(100);
            _running = true;
            _botThread = new Thread(MainLoop);
            _botThread.Start();
        }
        private void MainLoop()
        {
            _currentPlayArea = new PlayArea(_gameInterfacer.ScreenShot(), _gemClassifier);
            while(_running)
            {
                //get new data
                _currentPlayArea.UpdateWithScreenshot(_gameInterfacer.ScreenShot());
                //get the best move
                Move bestMove = _currentPlayArea.GetBestMove(1);
                if (bestMove.ValidMove)
                {
                    //execute the move
                    Point from = _currentPlayArea.GemSlots[bestMove.FromRow, bestMove.FromColumn].Rectangle.Center();
                    Point to = _currentPlayArea.GemSlots[bestMove.ToRow, bestMove.ToColumn].Rectangle.Center();
                    //click on the first gem
                    _gameInterfacer.Click(from.X, from.Y);
                    //maybe put a sleep here ?
                    //click on the second gem, initiating the swap
                    _gameInterfacer.Click(to.X, to.Y);
                }
                //wait then repeat
                Thread.Sleep(50);
            }
        }
        public void StopBot()
        {
            _botThread.Abort();
            _running = false;
        }
    }
}
