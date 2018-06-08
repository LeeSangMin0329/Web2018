using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class Timer : MonoBehaviour
    {
        #region Public Properties

        [Tooltip("Game Timer Text UI")]
        public Text TimerText;

        public int CurrentTime { get { return _min * 60 + (int)_sec; } }

        
        #endregion

        #region Private Properties

        // Timer
        private float _sec = 10;
        private int _min = 0;
        private bool _enable = false;


        #endregion

        #region Public Methods

        public void SetTimer(int sec)
        {
            _min = sec / 60;
            _sec = sec % 60;
            _enable = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Timer time down.
        /// If time 00 : 00, Call 'OnTimerOver()' one time.
        /// </summary>
        private void TimerUpdate()
        {
            if (_sec > 0)
            {
                _sec -= Time.deltaTime;
            }
            // minute down
            else
            {
                _sec = 0;
                if (_min > 0)
                {
                    _min--;
                    _sec = 60;
                }
                else
                {
                    _enable = false;
                    // Timer end 00 : 00
                    OnTimerOver();
                }
            }

            TimerText.text = string.Format("{0:D2} : {1:D2}", _min, (int)_sec);
        }

        /// <summary>
        /// #Critical
        /// Dependency UIManager on Callback
        /// </summary>
        private void OnTimerOver()
        {
            UIManager.Instance.OnTimerOver();
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            if (TimerText == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Timer Text on MafiaManager", this);
            }
            SetTimer(10);
        }

        // Update is called once per frame
        void Update()
        {
            if (_enable)
            {
                TimerUpdate();
            }
        }

        #endregion
    }
}