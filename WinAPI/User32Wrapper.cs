using System;
using System.Runtime.InteropServices;

namespace WinAPI
{
    public static class GetWindowLongEnum
    {
        public const int ExStyle = -20;
    }

    public static class WindowsExtendedStyle
    {
        public const int Transparent = 0x20;
        public const int Layered = 0x80000;
    }

    public static class LayeredWindowAttribute
    {
        public const int Alpha = 0x2;
    }

    public class User32Wrapper
    {
        [DllImport("User32.dll", EntryPoint = "GetWindowLong")]
        internal static extern int GetWindowLong(IntPtr hWnd, int getWindowLongEnum);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, int getWindowLongEnum, int windowsExtendedStyle);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern Boolean SetLayeredWindowAttributes(IntPtr hWnd, int key, Byte alpha, int layeredWindowAttribute);

    }
}
