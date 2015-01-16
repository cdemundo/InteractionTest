using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interaction
{
    class ImageSearch
    {
        private string _imagePath = "..\\..\\templates\\";

        internal Bitmap TemplateBitmap(string imageName)
        {
            System.Drawing.Bitmap template = (Bitmap)Bitmap.FromFile(_imagePath + imageName +".jpg");
            return template;
        }
        
        internal Point FindImage(Bitmap bigImage, string smallImage)
        {
            Stopwatch timePerSearch = Stopwatch.StartNew();
            Point whereToClick = new Point(); //will store location of where the image is found in the source
            Random rand = new Random(); //to randomize where the mouse will click (within the bounds of the template image)

            try
            {
                //Bitmap sourceImage = (Bitmap)Bitmap.FromFile(bigImage);
                Bitmap sourceImage = (Bitmap)bigImage;
                System.Drawing.Bitmap template = (Bitmap)Bitmap.FromFile(_imagePath + smallImage +".jpg");

                ///****************temporary to see what the screenshots look like*******************************
                sourceImage.Save("c:\\temp\\sourceImage.bmp", ImageFormat.Bmp);
                template.Save("c:\\temp\\template.bmp", ImageFormat.Bmp);

                // create template matching algorithm's instance
                // (set similarity threshold to 95.1%)
                ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.98f);

                // find all matchings with specified above similarity
                TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);

                //lock found data
                BitmapData data = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadWrite, sourceImage.PixelFormat);

                //find each match above 98% similarity
                foreach (TemplateMatch m in matchings)
                {
                    //store upper left point of selection (where the match is in the source)
                    //whereToClick.X = m.Rectangle.X + GlobalRandom.Next(0, template.Width);
                    whereToClick.X = m.Rectangle.X;
                    //whereToClick.Y = m.Rectangle.Y + GlobalRandom.Next(0, template.Height);
                    whereToClick.Y = m.Rectangle.Y;

                    Console.WriteLine("{0}, {1}", whereToClick, m.Similarity);
                }
                sourceImage.UnlockBits(data);
                timePerSearch.Stop();
                Console.WriteLine(timePerSearch.Elapsed.ToString());
            }
            catch (UnsupportedImageFormatException)
            {
                throw new ETAMarketOrderFailedException("Template image is an unsupported format - must be 24bppRgb");
            }
            catch (InvalidImagePropertiesException)
            {
                throw new ETAMarketOrderFailedException("Template image is larger than source - must be smaller than or equal to source.");
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new ETAMarketOrderFailedException("Template image for " + smallImage + " was not found.");
            }

            return whereToClick;
        }

        /// <summary>
        /// Resizes image by half - optimizes search time
        /// </summary>
        internal static Bitmap ResizeImage(Bitmap image)
        {
            Bitmap resized = new Bitmap(image, new Size(image.Width / 2, image.Height / 2));
            return resized;
        }

        internal static Bitmap ConvertTo24bpp(System.Drawing.Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }
    }
}
