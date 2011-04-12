using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace CodeSoda.Impression.LoadTests
{
    [TestFixture]
    public class LoadTest
    {
        private int count;

        [SetUp]
        public void setup()
        {
            
        }

        [Test]
        public void BasicLoad()
        {
            for (int times = 0; times < 10; times++)
            {
                count = 0;

                for (int i = 0; i < 1000; i++)
                {
                    count++;
                    new Thread(LoadMeUp1).Start();
                   // Debug.WriteLine("Progrssive Count = " + count);
                }
                //Debug.WriteLine("Final Count = " + count);
            }

            //GC.Collect();
        }

        private void LoadMeUp1()
        {
            PropertyBag bag = new PropertyBag();
            bag.Add("One", 1);
            bag.Add("Two", 2);
            bag.Add("Three", 3);
            bag.Add("Four", 4);
            bag.Add("Five", 5);
            string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
            string templatePath = Path.Combine(currentFolder, "../templates/SimpleIfElseIfElseEndIf.htm");
            ImpressionEngine.Create(bag).Run(templatePath);
            count--;
        }

        private void LoadMeUp2()
        {
            
        }
    }
}
