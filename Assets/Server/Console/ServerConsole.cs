using System;
using System.Collections.Generic;
using System.Linq;
using Decay.Server.ConsoleApp;
using Fusion;
using UnityEngine;

namespace Server.Console {
    public class ServerConsole : MonoBehaviour {

        private readonly ConsoleWindow _console = new ConsoleWindow();
        private readonly ConsoleInput _input = new ConsoleInput();

        public static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();
        private float _nextUpdate;
        private NetworkRunner _runner;

        /// <summary>
        /// Create console window, register callbacks.
        /// </summary>
        private void Awake() {
            DontDestroyOnLoad(gameObject);

            _console.Initialize();
            _console.SetTitle("Dedicated Server");

            _input.OnInputText += OnInputText;

            _runner = FindObjectOfType<NetworkRunner>();
            
            Application.logMessageReceived += HandleLog;
        }

        private void Update() {
            if ( _runner == null )
                _runner = FindObjectOfType<NetworkRunner>();

            UpdateStatus();
            _input.Update();

            while ( ExecuteOnMainThread.Count > 0 ) {
                ExecuteOnMainThread.Dequeue().Invoke();
            }
        }

        private int PlayersOnline => _runner == null ? 0 : _runner.ActivePlayers.Count();
        private int MaxPlayers => _runner == null ? 0 : _runner.Simulation.MaxConnections;
        
        private void UpdateStatus() {
            if ( _nextUpdate > (double) Time.realtimeSinceStartup )
                return;
            _nextUpdate = Time.realtimeSinceStartup + 0.33f;
            if ( !_input.valid )
                return;
            
            var str3 = $"{1f / Time.deltaTime}fps";
            var str5 = str3.PadLeft(_input.lineWidth - 1);
            
            var str2 = $"Online [{PlayersOnline}/{MaxPlayers}]";
            var str6 = str2 + (str2.Length < str5.Length ? str5.Substring(str2.Length) : "");

            _input.statusText[0] = "";
            _input.statusText[1] = str6;
            _input.statusText[2] = "";
        }

        private void OnDestroy() => _console.Shutdown();

        private static void Write(string message, LogType level = LogType.Log) =>
            System.Console.WriteLine($"[{DateTime.Now}] {message}");
        public static void Clear() => System.Console.Clear();

        /// <summary>
        /// Text has been entered into the console.
        /// Run it as a console command.
        /// </summary>
        /// <param name="fullCommand"></param>
        private void OnInputText(string fullCommand) {
            //Terminal.Execute(fullCommand);
        }

        /// <summary>
        /// Debug.Log callback
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void HandleLog(string message, string stackTrace, LogType type) {
            System.Console.ForegroundColor = type switch {
                LogType.Warning => ConsoleColor.Yellow,
                LogType.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            _input.ClearLine(_input.statusText.Length);
            Write(message, type);
            _input.RedrawInputLine();
        }
    }
}