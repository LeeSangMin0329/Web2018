using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Sangmin.Web2018
{
    public class GameManager : Photon.PunBehaviour
    {
        #region Public Properties

        // Singleton
        static public GameManager Instance;

        #endregion

        #region Public Variable

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public bool IsLevelLoad = true;

        #endregion
        
        #region Private Methods

        // player count to different scene load
        private void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading level : " + PhotonNetwork.room.PlayerCount);
            // Scene name is Room for [PlayerCount]
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
        }

        #endregion

        #region Photon.PunBehaviour CallBacks

        private void Start()
        {
            Instance = this;

            if(playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if(PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);

                    // spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    // #Caution
                    // instantiate position use hard coding!
                    PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
                }
            }
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            Debug.Log("OnPhotonPlayerConnected() " + newPlayer.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);

                if (!IsLevelLoad)
                {
                    return;
                }
                LoadArena(); // master client load scene
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + otherPlayer.NickName);

            if(PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient" + PhotonNetwork.isMasterClient);

                if (!IsLevelLoad)
                {
                    return;
                }
                LoadArena();
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0); // first scene
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        #endregion
    }
}
