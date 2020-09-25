using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageComparison;

// Created in 2012 by Jakob Krarup (www.xnafan.net).
// Use, alter and redistribute this code freely,
// but please leave this comment :)

namespace ConsoleComparison
{

    /// <summary>
    /// Console program which compares two iages and returns the difference in percentage as an errorlevel between zero and a hundred (both included).
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            var prefix = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            
            var p1 = Path.Join(prefix, "Screenshot from 2020-02-16 11-35-07.png");
            var p2 = Path.Join(prefix, "rr-parse-table.png");

            Console.WriteLine($"Diff: {Image.FromFile(p1).PercentageDifference(Image.FromFile(p2))}");
            Console.WriteLine($"Diff: {Image.FromFile(p1).BhattacharyyaDifference(Image.FromFile(p2))}");
            return 0;
        }
    }
}
