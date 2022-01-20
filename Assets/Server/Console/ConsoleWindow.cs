using System;
using System.IO;
using System.Runtime.InteropServices;
using Server.Console;
using UnityEngine;

namespace Decay.Server.ConsoleApp {
    /// <summary>
    /// Creates a console window that actually works in Unity
    /// You should add a script that redirects output using Console.Write to write to it.
    /// </summary>
    public class ConsoleWindow {
        private TextWriter _oldOutput;

        public void Initialize() {
            //
            // Attach to any existing consoles we have
            // failing that, create a new one.
            //
            if (!AttachConsole(0x0ffffffff)) { AllocConsole(); }

            _oldOutput = Console.Out;

            try {
                var stdHandle = GetStdHandle(StdOutputHandle);
                var safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
                var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                var encoding = System.Text.Encoding.ASCII;

                // Exit handle
                _eventHandle = ConsoleEventCallback;
                SetConsoleCtrlHandler(_eventHandle, true);

                var standardOutput = new StreamWriter(fileStream, encoding) {
                    AutoFlush = true
                };

                Console.SetOut(standardOutput);
            } catch (Exception e) {
                Debug.Log("Couldn't redirect output: " + e.Message);
            }
        }

        private bool ConsoleEventCallback(int eventType) {
            if(eventType == 2) {
                ServerConsole.ExecuteOnMainThread.Enqueue(Application.Quit);
            }
            return false;
        }

        public void Shutdown() {
            Console.SetOut(_oldOutput);
            FreeConsole();
        }

        public void SetTitle(string strName) {
            SetConsoleTitle(strName);
        }

        private const int StdOutputHandle = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);

        static ConsoleEventDelegate _eventHandle;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}