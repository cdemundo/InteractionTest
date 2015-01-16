using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InputManager;
using System.Threading;

namespace Interaction
{
    public class GUIInteraction
    {
        public static void DoMarket(string itemToSearch, bool buySell, int quantity)
        {
                /*List<string> stuff = new List<string> { "dominix", "naga", "megathron" };
                DumpMarket(stuff);*/
            
                Point whereToClick;
                try
                {
                    IsMarketOpen();
                }
                catch (ETAImageSearchFailedException)
                {
                    OpenMarketWindow();
                }

                ActivateMarketSearch();

                TextEntry(itemToSearch);

                //press enter to search for item after typing
                Keyboard.KeyPress(Keys.Enter);
                Thread.Sleep(250);
                Keyboard.KeyUp(Keys.Enter);
                Thread.Sleep(250);
            
                whereToClick = FindOnScreen(itemToSearch);

                if (whereToClick.X != 0)
                {
                    MoveMouse(whereToClick.X, whereToClick.Y);
                    LeftMouseClick();
                }

                /*whereToClick = FindOnScreen("placebuyorder");

                if (whereToClick.X != 0)
                {
                    MoveMouse(whereToClick.X, whereToClick.Y);
                    LeftMouseClick();
                }*/
        }

        public static void DumpMarket(List<string> itemsToDump)
        {
            Point whereToClick; 

            try
            {
                IsMarketOpen();
            }
            catch (ETAImageSearchFailedException)
            {
               OpenMarketWindow();
            }

            foreach (string s in itemsToDump)
            {
                ActivateMarketSearch();
                TextEntry(s);

                //press enter to search for item after typing
                Keyboard.KeyPress(Keys.Enter);
                Thread.Sleep(250);
                Keyboard.KeyUp(Keys.Enter);
                Thread.Sleep(250);

                whereToClick = FindOnScreen(s);

                if (whereToClick.X != 0)
                {
                    MoveMouse(whereToClick.X, whereToClick.Y);
                    LeftMouseClick();
                }

                whereToClick = FindOnScreen("exporttofile");

                if (whereToClick.X != 0)
                {
                    MoveMouse(whereToClick.X, whereToClick.Y);
                    LeftMouseClick();
                }

                Thread.Sleep(500);
            }
        }

        private static void TextEntry(string textToEnter)
        {
            Keys key;
            for (int i = 0; i < textToEnter.Length; i++)
            {
                Enum.TryParse(textToEnter[i].ToString(), true, out key);
                Keyboard.KeyDown(key);
                Thread.Sleep(250);
                Keyboard.KeyUp(key);
            }
        }

        private static void IsMarketOpen()
        {
            ImageSearch search = new ImageSearch();
            NativeMethods nm = new NativeMethods();

            //take a screenshot of EVE - get handle for process first
            Bitmap screen = nm.CaptureScreen("exefile");

            //find search bar
            Point whereToClick = search.FindImage(screen, "marketwindow");
        }

        /// <summary>
        /// Finds a template in the eve window, moves the mouse to a random position inside of it and left clicks
        /// </summary>
        private static Point FindOnScreen(string thingToSearch)
        {
            ImageSearch search = new ImageSearch();
            NativeMethods nm = new NativeMethods();
            Point whereToClick = new Point();

            try
            {
                RECT rect = nm.FindWindowInScreen("exefile");

                //take a screenshot of EVE - get handle for process first
                Bitmap screen = nm.CaptureScreen("exefile");

                Bitmap template = search.TemplateBitmap(thingToSearch);

                //find item bar
                int count = 0;

                whereToClick = search.FindImage(screen, thingToSearch);

                if (whereToClick.X == 0)
                {
                    if (count < 3)
                    {
                        whereToClick = search.FindImage(screen, thingToSearch);
                        count++;
                    }
                    else
                    {
                        throw new ETAImageSearchFailedException("Location of template not found.");
                    }
                }

                //return coordinates to original size and map to location in screen. will hit the 
                //clear button if there is a search saved
                switch (thingToSearch)
                {
                    case "searchbar" :
                        //searchbar jpeg finds the search button - needs to move to the left to activate the textbox (subtract instead of add)
                        whereToClick.X = rect.Left + (whereToClick.X * 2 - GlobalRandom.Next(0, template.Width));
                        whereToClick.Y = rect.Top + (whereToClick.Y * 2 + GlobalRandom.Next(3, template.Height));
                        return whereToClick;
                    default :
                        whereToClick.X = rect.Left + (whereToClick.X * 2 + GlobalRandom.Next(5, template.Width));
                        whereToClick.Y = rect.Top + ((whereToClick.Y * 2) + (template.Height + GlobalRandom.Next(5, 8)));
                        return whereToClick;
                }
            }
            catch (ETAMarketOrderFailedException)
            {
                //eve was not running
                whereToClick.X = 0;
                return whereToClick;
            }
        }

        /// <summary>
        /// Activates EVE Window and sends Alt-R to open Market Window
        /// </summary>
        private static void OpenMarketWindow()
        {
            NativeMethods nm = new NativeMethods();
            nm.BringWindowToFront("exefile");

            //find where eve is
            RECT rect = nm.FindWindowInScreen("exefile");

            MoveMouse((rect.Left + GlobalRandom.Next(400, 600)), (rect.Top + GlobalRandom.Next(300, 500)));

            Keyboard.KeyDown(Keys.LMenu);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.R);
            Thread.Sleep(500);

            Keyboard.KeyUp(Keys.LMenu);
            Keyboard.KeyUp(Keys.R);
        }

        /// <summary>
        /// Checks if there is a search term in the market window and clears it, then activates the search bar
        /// </summary>
        private static void ActivateMarketSearch()
        {
            Point whereToClick;
            try
            {
                whereToClick = FindOnScreen("searchbar");
                if (whereToClick.X != 0)
                {
                    MoveMouse(whereToClick.X, whereToClick.Y);
                    LeftMouseClick();
                    LeftMouseClick();
                }
            }
            catch (ETAImageSearchFailedException)
            {
                //this means the clearmarket.jpg template could not be found - must be no search term currently
            }
        }

        private static void LeftMouseClick()
        {
            Mouse.PressButton(Mouse.MouseKeys.Left);
            Thread.Sleep(500);
        }

        //This simulates a left mouse click
        private static void MoveMouse(int xpos, int ypos)
        {
            Mouse.Move(xpos, ypos);
            Thread.Sleep(GlobalRandom.Next(250, 500));
        }
    }
}
