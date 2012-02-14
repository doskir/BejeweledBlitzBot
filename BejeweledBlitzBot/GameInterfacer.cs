using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BejeweledBlitzBot
{
    internal class GameInterfacer
    {
        public GameInterfacer(WebBrowser webBrowser)
        {
            FlashHandle = GetFlashObjectHandle(webBrowser);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public enum PostMessageFlags
        {
            MK_LBUTTON = 0x0001,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200
        }

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        // ReSharper restore InconsistentNaming
        public static IntPtr FlashHandle;

        public void Click(int x, int y)
        {
            var pos = new IntPtr(y*0x10000 + x);
            SendMessage(FlashHandle, (uint) PostMessageFlags.WM_LBUTTONDOWN,
                        new IntPtr((int) PostMessageFlags.MK_LBUTTON), pos);
            System.Threading.Thread.Sleep(10);
            SendMessage(FlashHandle, (int) PostMessageFlags.WM_LBUTTONUP,
                        new IntPtr((int) PostMessageFlags.MK_LBUTTON),
                        pos);
            System.Threading.Thread.Sleep(10);
        }

        public static void DragDrop(int srcX, int srcY, int destX, int destY)
        {
            var srcPos = new IntPtr(srcY*0x10000 + srcX);
            var destPos = new IntPtr(destY*0x10000 + destX);
            //pick up
            SendMessage(FlashHandle, (uint) PostMessageFlags.WM_LBUTTONDOWN, IntPtr.Zero, srcPos);
            System.Threading.Thread.Sleep(10);

            //drop
            SendMessage(FlashHandle, (uint) PostMessageFlags.WM_LBUTTONUP, IntPtr.Zero, destPos);
            System.Threading.Thread.Sleep(10);
        }

        public Image<Bgr, byte> ScreenShot()
        {
            return ScreenShot(FlashHandle);
        }
        public Image<Bgr, byte> ScreenShot(IntPtr handle)
        {
            Bitmap frame = new Bitmap(760, 596);
            Graphics g = Graphics.FromImage(frame);
            IntPtr dc = g.GetHdc();
            PrintWindow(handle, dc, 1);
            g.ReleaseHdc();
            //we have the bitmap now
            //turn it into an Image for emgu
            BitmapData bmpData = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.ReadWrite,
                                                PixelFormat.Format24bppRgb);

            Image<Bgr, byte> tempImage = new Image<Bgr, byte>(frame.Width, frame.Height, bmpData.Stride, bmpData.Scan0);
            //to prevent any corrupted memory errors that crop up for some reason
            Image<Bgr, byte> image = tempImage.Clone();
            frame.UnlockBits(bmpData);
            //dispose all unused image data to prevent memory leaks
            frame.Dispose();
            tempImage.Dispose();
            return image;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public IntPtr GetFlashObjectHandle(WebBrowser webBrowser)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            EnumWindowsProc childProc = new EnumWindowsProc(EnumWindow);
            EnumChildWindows(webBrowser.Handle, childProc, GCHandle.ToIntPtr(listHandle));
            foreach (IntPtr ptr in result)
            {
                //only one of the handles will be correct and its class name will be "Internet Explorer_Server"
                //all other handles will ignore input or at least not forward it to the game
                if (IsIEServerWindow(ptr))
                {
                    return ptr;
                }
            }
            return IntPtr.Zero;
        }
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }

        #region GetClassName

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        private static bool IsIEServerWindow(IntPtr hWnd)
        {
            StringBuilder className = new StringBuilder(100);
            //Get the window class name
            int nRet = GetClassName(hWnd, className, className.Capacity);
            if (nRet != 0)
            {
                return (string.Compare(className.ToString(), "Internet Explorer_Server", true, CultureInfo.InvariantCulture) == 0);
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
