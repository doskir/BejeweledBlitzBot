﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using NUnit.Framework;

namespace BejeweledBlitzBot
{
    [TestFixture]
    public class TestSimpleGemClassifier
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
        public void SimpleGemClassifierTest()
        {
            SimpleGemClassifier gemClassifier = new SimpleGemClassifier();
            int testedGems = 0;
            int correctGems = 0;
            foreach(string directory in System.IO.Directory.GetDirectories("..\\..\\TestGems"))
            {
                string expectedColorString = System.IO.Path.GetFileName(directory);
                if (expectedColorString == "Nothing" || expectedColorString == "Overlaid" || expectedColorString == "Moving")
                    continue;
                GemColor expectedColor = (GemColor) Enum.Parse(typeof(GemColor),expectedColorString);
                foreach(string gemPath in System.IO.Directory.GetFiles(directory))
                {
                    Gem gem = gemClassifier.ClassifyGem(new Image<Bgr, byte>(gemPath));
                    if (gem.Color == expectedColor)
                        correctGems++;
                    else
                    {
                        Console.Error.WriteLine("Misclassified: {0} a {1} gem as a {2} gem", gemPath, expectedColor,
                                                gem.Color);
                        gemClassifier.ClassifyGem(new Image<Bgr, byte>(gemPath));
                    }
                    testedGems++;
                }
            }
            //we aim for 100% precision
            Console.WriteLine("{0} of {1} gems were correctly classified.", correctGems, testedGems);
            Assert.That(correctGems == testedGems);
        }

    }
}