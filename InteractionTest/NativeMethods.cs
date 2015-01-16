using InputManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Interaction
{
    //this is shared across classes to record the location of the EVE application in the screen
    
    /*internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }*/

    class NativeMethods
    {
        #region DllImports 

        //restore window
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //set active window (bring to front)
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
                
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        //for mouse clicks
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, uint dwExtraInf);

        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        //get location of window in screen/take screenshot
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        //get state of window (maximized v minimized)
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const UInt32 WS_MINIMIZE = 0x20000000;

        #endregion 

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="processName">The name of the process to capture.</param>
        /// <returns></returns>

        internal Bitmap CaptureScreen(string processName)
        {
            RECT rc;
            IntPtr windowHandle = GetHandle(processName);
            BringWindowToFront(processName);
            GetWindowRect(windowHandle, out rc);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(windowHandle, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            bmp = ImageSearch.ConvertTo24bpp(ImageSearch.ResizeImage(bmp));

            bmp.Save("c:\\temp\\sourceimage.bmp", ImageFormat.Bmp);

            return bmp;
        }

        internal void BringWindowToFront(string processName)
        {
            IntPtr windowHandle = GetHandle(processName);

            //restore window if the process was found
            ShowWindow(windowHandle, 1);
            //set window as active
            SetForegroundWindow(windowHandle);

        }

        internal RECT FindWindowInScreen(string processName)
        {
            RECT rect = new RECT();
            
            //get handle for window
            IntPtr windowHandle = GetHandle(processName);
            
            //find the coordinates in the window
            
            GetWindowRect(windowHandle, out rect);
            
            return rect;
        }

        internal bool CheckMinimized(string processName)
        {
            IntPtr windowHandle = GetHandle(processName);
            int style = GetWindowLong(windowHandle, -16);
            if ((style & WS_MINIMIZE) == WS_MINIMIZE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Gets handle for process for other methods - error checking, throws exception if process isn't found
        internal IntPtr GetHandle(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            IntPtr windowHandle;

            try
            {
                //get the handle of the process
                windowHandle = processes[0].MainWindowHandle;
            }
            catch (IndexOutOfRangeException) //no processes found with processName
            {
                throw new ETAMarketOrderFailedException(processName + " was not found running.");
            }

            return windowHandle;
        }

        //simulate keypress
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
    }
}
