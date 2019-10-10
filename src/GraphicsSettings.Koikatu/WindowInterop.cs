using System;
using System.Collections;
using UnityEngine;

namespace KeelPlugins
{
    internal static class WindowInterop
    {
        private static WinAPI.WindowStyleFlags backupStandard;
        private static WinAPI.WindowStyleFlags backupExtended;
        private static bool backupDone = false;

        public static void MakeBorderless(MonoBehaviour coroutineHost)
        {
            if(Screen.fullScreen)
                SetResolutionCallback(coroutineHost, Screen.width, Screen.height, false, MakeBorderless);
            else
                MakeBorderless();
        }

        private static void MakeBorderless()
        {
            var hwnd = WinAPI.GetActiveWindow();

            if(!backupDone)
            {
                backupStandard = WinAPI.GetWindowLongPtr(hwnd, WinAPI.WindowLongIndex.Style);
                backupExtended = WinAPI.GetWindowLongPtr(hwnd, WinAPI.WindowLongIndex.ExtendedStyle);
                backupDone = true;
            }

            var newStandard = backupStandard
                              & ~(WinAPI.WindowStyleFlags.Caption
                                 | WinAPI.WindowStyleFlags.ThickFrame
                                 | WinAPI.WindowStyleFlags.SystemMenu
                                 | WinAPI.WindowStyleFlags.MaximizeBox // same as TabStop
                                 | WinAPI.WindowStyleFlags.MinimizeBox // same as Group
                              );

            var newExtended = backupExtended
                              & ~(WinAPI.WindowStyleFlags.ExtendedDlgModalFrame
                                 | WinAPI.WindowStyleFlags.ExtendedComposited
                                 | WinAPI.WindowStyleFlags.ExtendedWindowEdge
                                 | WinAPI.WindowStyleFlags.ExtendedClientEdge
                                 | WinAPI.WindowStyleFlags.ExtendedLayered
                                 | WinAPI.WindowStyleFlags.ExtendedStaticEdge
                                 | WinAPI.WindowStyleFlags.ExtendedToolWindow
                                 | WinAPI.WindowStyleFlags.ExtendedAppWindow
                              );

            var width = Screen.width;
            var height = Screen.height;

            WinAPI.SetWindowLongPtr(hwnd, WinAPI.WindowLongIndex.Style, newStandard);
            WinAPI.SetWindowLongPtr(hwnd, WinAPI.WindowLongIndex.ExtendedStyle, newExtended);
            WinAPI.SetWindowPos(hwnd, 0, 0, 0, width, height, WinAPI.SetWindowPosFlags.NoMove);
        }

        public static void MakeWindowed()
        {
            RestoreBorder();
            Screen.SetResolution(Screen.width, Screen.height, false);
        }

        public static void MakeFullscreen()
        {
            RestoreBorder();
            Screen.SetResolution(Screen.width, Screen.height, true);
        }

        private static void RestoreBorder()
        {
            if(backupDone)
            {
                var hwnd = WinAPI.GetActiveWindow();
                WinAPI.SetWindowLongPtr(hwnd, WinAPI.WindowLongIndex.Style, backupStandard);
                WinAPI.SetWindowLongPtr(hwnd, WinAPI.WindowLongIndex.ExtendedStyle, backupExtended);
            }
        }

        public static void SetResolutionCallback(MonoBehaviour coroutineHost, int width, int height, bool fullscreen, Action callback)
        {
            Screen.SetResolution(width, height, fullscreen);
            coroutineHost.StartCoroutine(WaitFrame());

            IEnumerator WaitFrame()
            {
                yield return null;
                callback();
            }
        }
    }
}
