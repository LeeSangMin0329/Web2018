// # Property name 'LocalPlayerInstance' -> 'LocalPlayerGameObject' (name ~Instance is ambiguous)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerManager : Photon.MonoBehaviour
    {

        #region Readonly Properties

        private float _moveSpeed = 5.0f;
        private float _rotateSpeed = 5.0f;
        private float _jumpSpeed = 5.0f;

        #endregion

        #region Public Properties

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The Player's UI GameObject Prefab")]
        public GameObject PlayerUiPrefab;

        // Current can move?
        public bool IsMoveable { get; private set; }

        // Player can move? (Use set this state move or not. Undo moveable set.)
        public bool BasicMoveabled { get; private set; }

        #endregion

        #region Private Properties

        private Transform _ownTransform;
        private Rigidbody _ownRigidbody;

        private Vector3 _movement;
        private Vector3 _direction;
        private float _deltaX;
        private float _deltaZ;
        private bool _isJumping = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Call when change state.
        /// #Caution
        /// If you want behaviour control when change state, you must call this method.
        /// Not 'SetMoveable'
        /// - "MafiaManager" Only can call this.
        /// </summary>
        /// <param name="value"></param>
        public void SetStateMoveable(bool value)
        {
            BasicMoveabled = value;
            SetMoveable(value);
        }

        /// <summary>
        /// Set move and animation enable
        /// </summary>
        /// <param name="value">enable</param>
        public void SetMoveable(bool value)
        {
            IsMoveable = value;
            CharacterAnimation _ca = GetComponent<CharacterAnimation>();
            if (_ca)
            {
                _ca.IsMoveable = value;
            }
        }

        /// <summary>
        /// Respawn Local Player where parameter 'position' surround.
        /// use viewID for select position.
        /// </summary>
        /// <param name="position"></param>
        public void ReSpawn(Vector3 position)
        {
            int viewID = LocalPlayerInstance.GetPhotonView().viewID;
            // Local Player Instance view Id 1001, 2001, ...
            viewID /= 1000;
            LocalPlayerInstance.transform.position = CustomMath.SnailPosition(position, viewID, 2f);
            LocalPlayerInstance.transform.LookAt(position);
        }

        #endregion

        #region RPC Methods
        
        /// <summary>
        /// Photon local viewID ==> ex) player 1 viewID = 1001, 1002
        /// so select random number (1 ~ playercount)
        /// than send to '[RPC]this.OtherPlayerTag()' this number.
        /// </summary>
        public void SetPlayerTag()
        {
            if (PhotonNetwork.isMasterClient)
            {
                int reaperID = UnityEngine.Random.Range(1, PhotonNetwork.room.PlayerCount);

                photonView.RPC("OtherPlayerTag", PhotonTargets.All, reaperID);
            }
        }

        /// <summary>
        /// If i'm player 1, LocalPlayerInstance.viewID == 1001
        /// (int)(viewID / 1000) == player number
        /// </summary>
        /// <param name="reaperID">random one (1 ~ playercount)</param>
        [PunRPC]
        private void OtherPlayerTag(int reaperID)
        {
            // ex) reaperID == 2, player 2 is "Reaper"
            if ((LocalPlayerInstance.GetPhotonView().viewID / 1000) == reaperID)
            {
                LocalPlayerInstance.tag = "Reaper";
            }
            else
            {
                LocalPlayerInstance.tag = "Victim";
            }
        }
        
        #endregion

        #region Pun.MonoBehabiour CallBacks

        // Use this for initialization
        void Awake()
        {
            _ownTransform = GetComponent<Transform>();
            _ownRigidbody = GetComponent<Rigidbody>();

            // #Inportant
            //used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            CameraFollow cameraFollow = gameObject.GetComponent<CameraFollow>();

            if (cameraFollow != null)
            {
                if (photonView.isMine)
                {
                    cameraFollow.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraFollow Component on playerPrefab", this);
            }

            // Create the UI
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab) as GameObject;
                _uiGo.SendMessage("SetTarget", GetComponent<CharacterStatus>(), SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            BasicMoveabled = true;
            IsMoveable = true;
        }

        // Update is called once per frame
        void Update()
        {
            // RPC separate
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }

            // [key input]
            _deltaX = Input.GetAxisRaw("Horizontal");
            _deltaZ = Input.GetAxisRaw("Vertical");

            //if (Input.GetButtonDown("Jump"))
            //{
            //    _isJumping = true;
            //}
        }

        private void FixedUpdate()
        {
            if (IsMoveable)
            {
                Move();
                Trun();
                //Jump();
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            // check if we are outside the Map and if it's the case, spawn around the center of the map in a safe zone.
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                // #Caution
                // hard coding!
                transform.position = new Vector3(0f, 5f, 0f);
            }

            // Create the UI
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab) as GameObject;
                _uiGo.SendMessage("SetTarget", GetComponent<CharacterStatus>(), SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }

        #endregion

        #region Logic Methods

        private void Trun()
        {
            if (_deltaX == 0 && _deltaZ == 0)
            {
                return;
            }

            _direction.Set(_deltaX, 0, _deltaZ);
            _direction = _direction.normalized;

            Quaternion newRotation = Quaternion.LookRotation(_direction);

            _ownRigidbody.rotation = Quaternion.Slerp(_ownRigidbody.rotation, newRotation, _rotateSpeed * Time.deltaTime);
        }

        private void Move()
        {
            if (_deltaX == 0 && _deltaZ == 0)
            {
                return;
            }

            _movement.Set(_deltaX, 0, _deltaZ);

            _movement = _movement.normalized * _moveSpeed * Time.deltaTime;
            _ownRigidbody.MovePosition(_ownTransform.position + _movement);
        }

        private void Jump()
        {
            if (!_isJumping)
            {
                return;
            }

            _ownRigidbody.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
            _isJumping = false;
        }

        #endregion
    }
}
