using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    [RequireComponent(typeof(PhotonView))]
    public class BroadcastChat : Photon.MonoBehaviour
    {
        #region Public Variables

        [Tooltip("The Player's Chat UI GameObject Prefab")]
        public GameObject PlayerChatPrefab;

        #endregion

        #region Private Variables

        private Chat _chatScript;

        private PlayerManager _playerManager;

        // Network trigger. Change when write chat, than use OnPhotonSerializeView.
        private bool _isUINotShowed = true;

        private bool _isEnabled = true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Set enable chatting
        /// </summary>
        /// <param name="value">enable chat</param>
        public void SetActiveChat(bool value)
        {
            _isEnabled = value;
        }

        #endregion

        #region Pun.MonoBehabiour CallBacks

        // Use this for initialization
        void Start()
        {
            if (PlayerChatPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerChatPrefab) as GameObject;
                _uiGo.SendMessage("SetTarget", GetComponent<CharacterStatus>(), SendMessageOptions.RequireReceiver);
                _chatScript = _uiGo.GetComponent<Chat>();
                _chatScript.TransferText = string.Empty;
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerSpeakPrefab reference on player Prefab.", this);
            }

            _playerManager = GetComponent<PlayerManager>();
            if (_playerManager == null)
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerManager reference on BroadcastChat.", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // RPC separate
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }
            
            // Disable chat
            if (!_isEnabled)
            {
                // When disable state. If UI Set View yet, erase this.
                if (!_isUINotShowed)
                {
                    _isUINotShowed = _chatScript.InputFieldActivate();
                }
                return;
            }

            if (_playerManager.BasicMoveabled)
            {
                // If chat ui enable, can't move character.
                if (_isUINotShowed)
                {
                    _playerManager.SetMoveable(true);
                }
                else
                {
                    _playerManager.SetMoveable(false);
                }
            }

            if (Input.GetButtonDown("Enter"))
            {
                if (_isUINotShowed = _chatScript.InputFieldActivate())
                {
                    photonView.RPC("SendChatToOtherPlayers", PhotonTargets.Others, _chatScript.TransferText);
                }
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            if (PlayerChatPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerChatPrefab) as GameObject;
                _uiGo.SendMessage("SetTarget", GetComponent<CharacterStatus>(), SendMessageOptions.RequireReceiver);
                _chatScript = _uiGo.GetComponent<Chat>();
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerSpeakPrefab reference on player Prefab.", this);
            }
        }

        #endregion

        #region RPC Methods

        /// <summary>
        /// Other client's my character view this message on chat.
        /// </summary>
        /// <param name="message"></param>
        [PunRPC]
        private void SendChatToOtherPlayers(string message)
        {
            _chatScript.TransferText = message;
            _chatScript.IsShowed = true;
        }

        #endregion
        
    }
}
