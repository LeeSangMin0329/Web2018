using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class CharacterAnimation : Photon.MonoBehaviour
    {
        #region Public Properties

        [Tooltip("If this value is false, Character animation not work.")]
        public bool IsMoveable = true;

        #endregion

        #region Private Properties

        private Animator _ownAnimator;

        private float _deltaX;
        private float _deltaZ;

        #endregion

        #region Public Methods

        public void SetDeath()
        {
            _ownAnimator.SetTrigger("Death");
        }

        #endregion

        #region Photon.MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            _ownAnimator = GetComponent<Animator>();
            if (!_ownAnimator)
            {
                Debug.LogError("PlayerAnimatorMassage is Missing Animator Component", this);
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
            if (!_ownAnimator)
            {
                return;
            }
            if (!IsMoveable)
            {
                _ownAnimator.SetFloat("Speed", 0);
                return;
            }

            // deal with Jumping
            if (Input.GetButtonDown("Jump"))
            {
                _ownAnimator.SetTrigger("Jump");
            }

            _deltaX = Input.GetAxis("Horizontal");
            _deltaZ = Input.GetAxis("Vertical");

            _ownAnimator.SetFloat("Speed", (_deltaZ * _deltaZ + _deltaX * _deltaX));
        }
        
        #endregion
    }
}
