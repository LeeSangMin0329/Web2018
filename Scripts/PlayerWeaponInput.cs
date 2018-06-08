using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    /// <summary>
    /// Just Network share perpose.
    /// separate Logic, collision and this.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PlayerWeaponInput : Photon.MonoBehaviour, IPunObservable
    {

        #region Private Properties

        private PlayerHitArea _pickWeapon;

        // this two value send PickWeapon on player prefab
        private bool _pick;
        private bool _drop;

        #endregion

        #region Photon.MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            _pickWeapon = GetComponentInChildren<PlayerHitArea>();
            if(_pickWeapon == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> _pickWeapon on WeaponPickDropContrll", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // synchronus
            _pickWeapon.Pick = _pick;
            _pickWeapon.Drop = _drop;

            // RPC separate
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }

            // [Input]
            if (Input.GetButtonDown("Z"))
            {
                _pick = true;
            }
            if (Input.GetButtonUp("Z"))
            {
                _pick = false;
            }

            if (Input.GetButtonDown("X"))
            {
                _drop = true;
            }
            if (Input.GetButtonUp("X"))
            {
                _drop = false;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(_pick);
                stream.SendNext(_drop);
            }
            else
            {
                // [#Issue 1] Invaild cast exception
                _pick = (bool)stream.ReceiveNext();
                _drop = (bool)stream.ReceiveNext();
            }
        }

        #endregion
    }
}