using Sketch.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using static UnityEngine.Rendering.DebugUI.MessageBox;

namespace BubbleFriend
{
    [AutoStaticsCleanup]
    public partial class SystemManager : MonoBehaviour
    {
        public static SystemManager Instance { private set; get; }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        // https://stackoverflow.com/a/34703664 and https://discussions.unity.com/t/how-do-you-reliably-the-hwnd-window-handle-of-the-games-own-window/699477/8
        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("Kernel32.dll")]
        static extern int GetCurrentThreadId();

        // https://www.pinvoke.net/default.aspx/user32.FlashWindowEx
        // Thanks Indra
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        static IntPtr GetWindowHandle()
        {
            IntPtr returnHwnd = IntPtr.Zero;
            var threadId = GetCurrentThreadId();
            EnumThreadWindows(threadId,
                (hWnd, lParam) => {
                    if (returnHwnd == IntPtr.Zero) returnHwnd = hWnd;
                    return true;
                }, IntPtr.Zero);
            return returnHwnd;
        }

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        private IntPtr _handle;


        private void Awake()
        {
            Instance = this;

            _handle = GetWindowHandle();

            Translate.Instance.SetLanguages(new Dictionary<string, string>()
            {
                { "English", "english" },
                { "Francais", "french" },
                { "Nederlands", "dutch" }
            });
#if !UNITY_EDITOR
            SetWindowPos(_handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
#endif
        }

        public void FlashWindow()
        {
            FlashWindow(_handle, true);
        }
    }
}
