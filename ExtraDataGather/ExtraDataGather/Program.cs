using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraDataGather
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter any number of space separated file names process: ");
            string filenamesInput = Console.ReadLine();
            string[] filenames = filenamesInput.Split(' ');

            foreach (string filename in filenames)
            {
                Console.WriteLine("Reading file: " + filename);
                ReplayReader reader = new ReplayReader(filename);
                reader.processAllEvents();
                Console.WriteLine("Reading current file complete.");
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
