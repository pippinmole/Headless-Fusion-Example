using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Server.Console;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server {
    public class HeadlessController : MonoBehaviour, INetworkRunnerCallbacks {
        [SerializeField] private NetworkRunner _runnerPrefab;

        private void Awake() {
            Application.targetFrameRate = 30;
            
            gameObject.AddComponent<ServerConsole>();
        }

        private async void Start() {
            // Quit if not in Headless mode
            if ( !HeadlessUtils.IsHeadlessMode() ) {
                Application.Quit(1);
            }

            // Get game scene to be loaded
            var sceneToLoad = HeadlessUtils.GetArg("-scene") ?? SceneManager.GetActiveScene().name;

            var scene = HeadlessUtils.GetArg("-scene");
            
            Debug.Log($"Starting Server");

            // Create a new Fusion Runner
            var runner = Instantiate(_runnerPrefab);

            // Basic Setup
            runner.name = $"DedicatedServer";
            runner.AddCallbacks(this); // register callbacks

            // Start the Server
            var result = await runner.StartGame(new StartGameArgs() {
                GameMode = GameMode.Server, // for dedicated servers,
                Scene = SceneManager.GetSceneByName(sceneToLoad).buildIndex,
                CustomLobbyName = "Default",
                SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            // Check if all went fine
            if ( result.Ok ) {
                Debug.Log($"Runner Start DONE");
            } else {
                // Quit the application if startup fails

                Debug.LogError($"Error while starting Server: {result.ShutdownReason}");

                // it can be used any error code that can be read by an external application
                // using 0 means all went fine
                Application.Quit(1);
            }
        }

        // Fusion INetworkRunnerCallbacks implementation

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
            Debug.LogWarning($"{nameof(OnShutdown)}: {nameof(shutdownReason)}: {shutdownReason}");

            // Quit normally
            Application.Quit(0);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) =>
            Debug.LogWarning($"{nameof(OnPlayerJoined)}: {nameof(player)}: {player}");

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) =>
            Debug.LogWarning($"{nameof(OnPlayerLeft)}: {nameof(player)}: {player}");

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token) =>
            Debug.LogWarning(
                $"{nameof(OnConnectRequest)}: {nameof(NetworkRunnerCallbackArgs.ConnectRequest)}: {request.RemoteAddress}");

        public void OnSceneLoadDone(NetworkRunner runner) =>
            Debug.LogWarning($"{nameof(OnSceneLoadDone)}: {nameof(runner.CurrentScene)}: {runner.CurrentScene}");

        public void OnSceneLoadStart(NetworkRunner runner) =>
            Debug.LogWarning($"{nameof(OnSceneLoadStart)}: {nameof(runner.CurrentScene)}: {runner.CurrentScene}");

        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    }
}