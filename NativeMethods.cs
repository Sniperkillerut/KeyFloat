using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KeyFloat {
    internal static class NativeMethods {

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        ////////////////////////blur


        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal static void EnableBlur(AccentState acc, Window win) {
            WindowInteropHelper windowHelper = new WindowInteropHelper(win);

            AccentPolicy accent = new AccentPolicy {
                AccentState = acc
            };

            int accentStructSize = Marshal.SizeOf(accent);

            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            WindowCompositionAttributeData data = new WindowCompositionAttributeData {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }


        internal enum AccentState {
            ACCENT_DISABLED = 1,
            ACCENT_ENABLE_GRADIENT = 0,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy {
            public AccentState AccentState;
#pragma warning disable CC0074 // Make field readonly
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
#pragma warning restore CC0074 // Make field readonly
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute {

            // ...
            WCA_ACCENT_POLICY = 19

            // ...
        }

    }
}
