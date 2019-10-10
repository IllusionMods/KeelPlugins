using System;
using System.Runtime.InteropServices;

namespace KeelPlugins
{
    internal static class WinAPI
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags wFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern WindowStyleFlags SetWindowLongPtr32(IntPtr hWnd, WindowLongIndex nIndex, WindowStyleFlags dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        public static extern WindowStyleFlags SetWindowLongPtr64(IntPtr hWnd, WindowLongIndex nIndex, WindowStyleFlags dwNewLong);

        public static WindowStyleFlags SetWindowLongPtr(IntPtr hWnd, WindowLongIndex nIndex, WindowStyleFlags dwNewLong)
        {
            return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern WindowStyleFlags GetWindowLongPtr32(IntPtr hWnd, WindowLongIndex nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern WindowStyleFlags GetWindowLongPtr64(IntPtr hWnd, WindowLongIndex nIndex);

        public static WindowStyleFlags GetWindowLongPtr(IntPtr hWnd, WindowLongIndex nIndex)
        {
            return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
        }

        public enum WindowLongIndex
        {
            Style = -16,
            ExtendedStyle = -20
        }

        [Flags]
        public enum WindowStyleFlags : uint
        {
            Overlapped = 0x00000000,
            Popup = 0x80000000,
            Child = 0x40000000,
            Minimize = 0x20000000,
            Visible = 0x10000000,
            Disabled = 0x08000000,
            ClipSiblings = 0x04000000,
            ClipChildren = 0x02000000,
            Maximize = 0x01000000,
            Border = 0x00800000,
            DialogFrame = 0x00400000,
            Vscroll = 0x00200000,
            Hscroll = 0x00100000,
            SystemMenu = 0x00080000,
            ThickFrame = 0x00040000,
            Group = 0x00020000,
            Tabstop = 0x00010000,

            MinimizeBox = 0x00020000,
            MaximizeBox = 0x00010000,

            Caption = Border | DialogFrame,
            Tiled = Overlapped,
            Iconic = Minimize,
            SizeBox = ThickFrame,
            TiledWindow = Overlapped,

            OverlappedWindow = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,
            ChildWindow = Child,

            ExtendedDlgModalFrame = 0x00000001,
            ExtendedNoParentNotify = 0x00000004,
            ExtendedTopmost = 0x00000008,
            ExtendedAcceptFiles = 0x00000010,
            ExtendedTransparent = 0x00000020,
            ExtendedMDIChild = 0x00000040,
            ExtendedToolWindow = 0x00000080,
            ExtendedWindowEdge = 0x00000100,
            ExtendedClientEdge = 0x00000200,
            ExtendedContextHelp = 0x00000400,
            ExtendedRight = 0x00001000,
            ExtendedLeft = 0x00000000,
            ExtendedRTLReading = 0x00002000,
            ExtendedLTRReading = 0x00000000,
            ExtendedLeftScrollbar = 0x00004000,
            ExtendedRightScrollbar = 0x00000000,
            ExtendedControlParent = 0x00010000,
            ExtendedStaticEdge = 0x00020000,
            ExtendedAppWindow = 0x00040000,
            ExtendedOverlappedWindow = ExtendedWindowEdge | ExtendedClientEdge,
            ExtendedPaletteWindow = ExtendedWindowEdge | ExtendedToolWindow | ExtendedTopmost,
            ExtendedLayered = 0x00080000,
            ExtendedNoinheritLayout = 0x00100000,
            ExtendedLayoutRTL = 0x00400000,
            ExtendedComposited = 0x02000000,
            ExtendedNoActivate = 0x08000000
        }

        [Flags]
        public enum SetWindowPosFlags
        {
            AsyncWindowPos = 0x4000,
            DeferBase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            NoActivate = 0x0010,
            NoCopyBits = 0x0100,
            NoMove = 0x0002,
            NoOwnerZOrder = 0x0200,
            NoReDraw = 0x0008,
            NoRePosition = 0x0200,
            NoSendChanging = 0x0400,
            NoSize = 0x0001,
            NoZOrder = 0x0004,
            ShowWindow = 0x0040
        }
    }
}
