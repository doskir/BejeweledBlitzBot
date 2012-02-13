using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using BejeweledBlitzBot.MoveFinder;

namespace BejeweledBlitzBot
{
    class Logic
    {
        private GameInterfacer _gameInterfacer;
        private ImageProcessing _imageProcessing = new ImageProcessing();
        private IGemClassifier _gemClassifier = new SimpleGemClassifier();
        private PlayArea _currentPlayArea;
        private IMoveFinder _moveFinder = new LongMatchFinder();
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
                //get the locked out positions
                List<Position> lockedOutPositions = _currentPlayArea.GetLockedOutPositions();
                //get the best move
                Move bestMove = _moveFinder.GetBestMove(_currentPlayArea.ToGemArray(), 1, lockedOutPositions);
                if (bestMove.ValidMove)
                {
                    //execute the move
                    Point from = _currentPlayArea.GemSlots[bestMove.From.Row, bestMove.From.Column].Rectangle.Center();
                    Point to = _currentPlayArea.GemSlots[bestMove.To.Row, bestMove.To.Column].Rectangle.Center();
                    //click on the first gem
                    _gameInterfacer.Click(from.X, from.Y);
                    //maybe put a sleep here ?
                    //click on the second gem, initiating the swap
                    _gameInterfacer.Click(to.X, to.Y);
                    if(bestMove.UsedPositions != null)
                    {
                        foreach(Position position in bestMove.UsedPositions)
                        {
                            _currentPlayArea.GemSlots[position.Row, position.Column].LockedUntil =
                                DateTime.Now.AddMilliseconds(500);
                        }
                    }
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
