using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageComparison
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare to</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static double PercentageDifference(this Image img1, Image img2, byte threshold = 3)
        {
            var differences = img1.GetDifferences(img2);

            var (height, width) = GetDimensions(img1, img2);

            var diffPixels = differences.Cast<int>().Count(b => b > threshold);

            return 1.0 * diffPixels / (height * width);
        }

        /// <summary>
        /// The Bhattacharyya difference (the difference between normalized versions of the histograms of both images)
        /// This tells something about the differences in the brightness of the images as a whole, not so much about where they differ.
        /// </summary>
        /// <param name="img1">The first image to compare</param>
        /// <param name="img2">The second image to compare</param>
        /// <returns>The difference between the images' normalized histograms</returns>
        public static double BhattacharyyaDifference(this Image img1, Image img2)
        {
            var img1GrayscaleValues = GetGrayScaleValues(new Bitmap(img1));
            var img2GrayscaleValues = GetGrayScaleValues(new Bitmap(img2));

            var (height, width) = GetDimensions(img1, img2);
            
            var normalizedHistogram1 = new double[width, height];
            var normalizedHistogram2 = new double[width, height];

            var histSum1 = img1GrayscaleValues.Cast<int>().Aggregate(0.0, (current, value) => current + value);
            var histSum2 = img2GrayscaleValues.Cast<int>().Aggregate(0.0, (current, value) => current + value);


            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    normalizedHistogram1[x, y] = img1GrayscaleValues[x, y] / histSum1;
                }
            }
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    normalizedHistogram2[x, y] = img2GrayscaleValues[x, y] / histSum2;
                }
            }

            var bCoefficient = 0.0;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var histSquared = normalizedHistogram1[x, y] * normalizedHistogram2[x, y];
                    bCoefficient += Math.Sqrt(histSquared);
                }
            }

            var dist1 = 1.0 - bCoefficient;
            dist1 = Math.Round(dist1, 8);
            var distance = Math.Sqrt(dist1);
            distance = Math.Round(distance, 8);
            return (float)distance;
        }

        public static (int height, int width) GetDimensions(Image img1, Image img2)
        {
            var height = Math.Min(img1.Height, img2.Height);
            var width = Math.Min(img1.Width, img2.Width);

            return (height, width);
        }

        /// <summary>
        /// Finds the differences between two images and returns them in a double array
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <returns>the differences between the two images as a double array</returns>
        public static int[,] GetDifferences(this Image img1, Image img2)
        {
            var thisOne = new Bitmap(img1);
            var theOtherOne = new Bitmap(img2);

            var (height, width) = GetDimensions(img1, img2);

            var differences = new int[width, height];
            var firstGray = thisOne.GetGrayScaleValues();
            var secondGray = theOtherOne.GetGrayScaleValues();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    differences[x, y] = Math.Abs(firstGray[x, y] - secondGray[x, y]);
                }
            }

            thisOne.Dispose();
            theOtherOne.Dispose();
            return differences;
        }

        /// <summary>
        /// Gets the lightness of the image in 256 sections (16x16)
        /// </summary>
        /// <param name="img">The image to get the lightness for</param>
        /// <returns>A double array (16x16) containing the lightness of the 256 sections</returns>
        public static int[,] GetGrayScaleValues(this Bitmap img)
        {
            using (var thisOne = img)
            {
                var grayScale = new int[img.Width, img.Height];

                for (var x = 0; x < img.Width; x++)
                {
                    for (var y = 0; y < img.Height; y++)
                    {
                        grayScale[x, y] = thisOne.GetPixel(x, y).ToArgb();
                    }
                }

                return grayScale;
            }
        }
    }
}