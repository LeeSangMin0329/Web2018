using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class Launcher : Photon.PunBehaviour
    {
        #region UI
        [Tooltip("The UI Panel to let the user enter name, connect and play")]
        public GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;
        #endregion

        #region Public Variables
        public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        public byte MaxPlayersPerRoom = 6;
        #endregion

        #region Private Variables
        private string _gameVersion = "2";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        private bool _isConnecting;
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // log print ( recommended - Informational )
            PhotonNetwork.logLevel = LogLevel;

            PhotonNetwork.autoJoinLobby = false;

            PhotonNetwork.automaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            // Keep track of the will to join a room,
            // because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            _isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // check if we are connected or not
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else // initiate connection to server
            {
                // start Connecting server on first time
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #endregion

        #region Photon.PunBehaviour callBacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");

            if (_isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room.
                //            If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");    
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("DemoAnimator/Launcher: OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.");
            PhotonNetwork.CreateRoom(null, roomOptions: new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, typedLobby: null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            if(PhotonNetwork.room.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room Level for Scene name
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }

        #endregion

    }
}
