using Sketch.Translation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend
{
    [AutoStaticsCleanup]
    public partial class SystemManager : MonoBehaviour
    {
        // https://stackoverflow.com/a/34703664 and https://discussions.unity.com/t/how-do-you-reliably-the-hwnd-window-handle-of-the-games-own-window/699477/8
        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("Kernel32.dll")]
        static extern int GetCurrentThreadId();

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


        private void Awake()
        {
            Translate.Instance.SetLanguages(new Dictionary<string, string>()
            {
                { "English", "english" },
                { "Francais", "french" }
            });
#if !UNITY_EDITOR
            SetWindowPos(GetWindowHandle(), HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
#endif
        }
    }
}
