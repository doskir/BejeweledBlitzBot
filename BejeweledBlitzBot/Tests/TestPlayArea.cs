using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using NUnit.Framework;

namespace BejeweledBlitzBot
{
    [TestFixture]
    public class TestPlayArea
    {

        [TestFixtureSetUp]
        public void SetupMethods()
        {
        }

        [TestFixtureTearDown]
        public void TearDownMethods()
        {
        }

        [SetUp]
        public void SetupTest()
        {
        }

        [TearDown]
        public void TearDownTest()
        {
        }

        [Test]
        public void GetBestMoveTest()
        {
            PlayArea playArea = new PlayArea(new Image<Bgr, byte>("testplayarea.png"), new SimpleGemClassifier());
            IMoveFinder moveFinder = new IdiotMoveFinder();
            int counter = 0;
            while(true)
            {
                Move bestMove = moveFinder.GetBestMove(playArea.ToGemArray(), 1);
                if (bestMove.ValidMove)
                {
                    counter++;
                    playArea.GemSlots[bestMove.FromRow, bestMove.FromColumn].Gem.Color = GemColor.Unknown;
                }
                else
                    break;
            }




        }

    }
}