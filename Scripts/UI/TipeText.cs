using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class TipeText : MonoBehaviour
    {
        #region Public Properties

        // Total tipe time.
        [SerializeField]
        public float TipeTime { get; private set; }

        // now is tiping?
        public bool Enabled { get; private set; }

        #endregion

        #region Private Properties

        // Target Text object.
        private Text _text;

        // Goal sentence.
        private string _fullText;

        // tipe time to one letter.
        private float _oneLetterTipeTime;

        private float _timer;
        private int _count;

        #endregion

        #region Public Methoeds

        /// <summary>
        /// Set Text and Start tipe that.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text, float tipeTime)
        {
            if(text.Length == 0)
            {
                _text.text = string.Empty;
                return;
            }

            TipeTime = tipeTime;
            _text.text = string.Empty;
            _fullText = text;
            _timer = 0;
            _count = 0;

            _oneLetterTipeTime = TipeTime / _fullText.Length;

            Enabled = true;
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Awake()
        {
            _text = GetComponent<Text>();
            if(_text == null)
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> Text reference on TipeText.", this);
            }

            Enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!Enabled)
            {
                return;
            }

            _timer -= Time.deltaTime;

            // Tipe one letter
            if(_timer <= 0)
            {
                _timer = _oneLetterTipeTime;
                _text.text += _fullText[_count];

                _count++;

                if(_count == _fullText.Length)
                {
                    Enabled = false;
                }
            }
        }

        #endregion
    }
}