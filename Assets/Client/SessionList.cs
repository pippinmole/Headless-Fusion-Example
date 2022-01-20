using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Client {
    public class SessionList : MonoBehaviour {

        private List<SessionInfo> _sessions = new List<SessionInfo>();

        private void OnEnable() {
            ClientBootstrap.SessionListUpdated += UpdateList;
        }

        private void OnDestroy() {
            ClientBootstrap.SessionListUpdated -= UpdateList;
        }

        private void UpdateList(List<SessionInfo> sessionInfos) {
            _sessions = sessionInfos;
        }

        private void OnGUI() {
            foreach ( var session in _sessions ) {
                
                if ( GUILayout.Button($"Join {session.Name}") ) {
                    var runner = FindObjectOfType<NetworkRunner>();
                    runner.StartGame(new StartGameArgs() {
                        GameMode = GameMode.Client,
                        SessionName = session.Name,
                        CustomLobbyName = "Default",
                        SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
                    });
                }
                
            }
        }
    }
}