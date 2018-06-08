using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class PlayerUI : MonoBehaviour
    {

        #region Public Properties

        [Tooltip("UI Text to display Player's name")]
        public Text PlayerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        public Slider PlayerHealthSlider;

        [Tooltip("Pixel offset from the player target")]
        public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region Private Properties

        private CharacterStatus _target;

        private float _characterHeight = 1f;
        private Transform _targetTransform;

        private Vector3 _targetPosition;

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns a Player Target to Follow and represent
        /// </summary>
        /// <param name="target">Target</param>
        public void SetTarget(CharacterStatus target)
        {
            if (target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayerMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            // Cache references efficiency
            _target = target;
            _targetTransform = _target.GetComponent<Transform>();

            if (PlayerNameText != null)
            {
                PlayerNameText.text = _target.photonView.owner.NickName;
            }

            if (!target.GetComponent<PhotonView>().isMine)
            {
                PlayerHealthSlider.gameObject.SetActive(false);
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        }

        // Update is called once per frame
        void Update()
        {
            // Reflect the Player Health
            if (PlayerHealthSlider != null)
            {
                PlayerHealthSlider.value = _target.HP * (1/_target.MaxHP);
            }

            // Destroy itself if the target is null, It;s a fail safe when Photon is destroying Instances of a Player over the network.
            if (_target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void LateUpdate()
        {
            // #Critical
            // Follow the Target GameObject on screen.
            if (_targetTransform != null)
            {
                _targetPosition = _targetTransform.position;
                _targetPosition.y += _characterHeight;

                this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + ScreenOffset;
            }
        }

        #endregion
    }
}