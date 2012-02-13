using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
            Bitmap frame = new Bitmap(760, 596);
            Graphics g = Graphics.FromImage(frame);
            IntPtr dc = g.GetHdc();
            PrintWindow(FlashHandle, dc, 1);
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
#if DEBUG
            foreach (IntPtr ptr in result)
            {

                Rectangle rectangle = GetWindowRectangle(ptr);
                Debug.WriteLine("{0}: W:{1} H:{2}", ptr, rectangle.Width, rectangle.Height);
            }
#endif
            //it looks like the last handle always points to the flash object with bejeweled blitz in it
            return result.Last();
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

        #region WindowInfo

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filler)
                : this() // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
            {
                cbSize = (UInt32) (Marshal.SizeOf(typeof (WINDOWINFO)));
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public Rectangle GetWindowRectangle(IntPtr handle)
        {
            WINDOWINFO info = new WINDOWINFO();
            info.cbSize = (uint) Marshal.SizeOf(info);
            GetWindowInfo(handle, ref info);
            Rectangle rectangle = new Rectangle(0, 0, info.rcClient.Right - info.rcClient.Left,
                                                info.rcClient.Bottom - info.rcClient.Top);
            return rectangle;
        }

        #endregion
    }
}
