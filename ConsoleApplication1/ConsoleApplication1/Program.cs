using System;
using System.Runtime.InteropServices; // for DllImport
using Accessibility;

namespace CSClient
{
    class CSClientProgram
    {
        [DllImport("user32.dll")]
        private static extern int GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string sClass, string sWindow);

        [DllImport("Oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(int hwnd, uint dwObjectID, byte[] riid,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        internal static Guid IID_IAccessible = new Guid(0x618736e0, 0x3c3d, 0x11cf, 0x81, 0x0c, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        [DllImport("IAccessible2Proxy.dll")]
        private static extern int attributes(string attributes);

        static void Main(string[] args)
        {
            var DesktopHandle = GetDesktopWindow();
            Console.WriteLine("Desktop Handle : " + new IntPtr(DesktopHandle).ToString("X") + "\n\n");

            int FxWinHandle = GetBrowserWindow("MozillaWindowClass");
            //int GCWinHandle = GetBrowserWindow("Chrome_WidgetWin_1");
            if (FxWinHandle == 0) return;   //didn't find browser window

            const uint OBJID_CLIENT = 0xFFFFFFFC;
            object pAcc = null;
            AccessibleObjectFromWindow(FxWinHandle, OBJID_CLIENT, IID_IAccessible.ToByteArray(), ref pAcc);
            IAccessible iAccessible = (IAccessible)pAcc;

            object pService = null;
            IntPtr IID_IServiceProvider = Marshal.GetIUnknownForObject(iAccessible);
            Guid myGuid = new Guid();
            int hr = iAccessible.QueryInterface(IID_IServiceProvider, ref pService);





        }



        private static int GetBrowserWindow(string browserClass)
        {
            int ret = FindWindow(browserClass, null);
            if (ret == 0)
            {
                Console.WriteLine("No " + browserClass + " Window Found.\nDo Something.");
                //return error
            }
            else
            {
                Console.WriteLine("Window handle for " + browserClass +
                                  " is : " + ret.ToString("X"));
            }
            return ret;
        }
    }
}