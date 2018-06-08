using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class UIManager : MonoBehaviour
    {
        #region Public Properties

        // Singleton
        public static UIManager Instance;

        [Tooltip("Day time victim's UI.")]
        public GameObject DayUI;

        [Tooltip("Day time reaper's UI.")]
        public GameObject DayUIReaper;
        
        [Tooltip("Victim's UI for night time. If change night time, set active this.")]
        public GameObject NightUI;

        [Tooltip("Reaper's UI for night time. If change night time, set active this.")]
        public GameObject NightUIReaper;

        [Tooltip("Game Rule Explain UI. Use when Game state 'Explain'")]
        public GameObject ExplainUI;

        [Tooltip("Win text to UI")]
        public GameObject WinUI;

        [Tooltip("Defeat text to UI")]
        public GameObject DefeatUI;

        public GameObject DayMark;
        public GameObject NightMark;

        #endregion

        #region Private Properties

        public Timer GameTimer { get; private set; }

        #endregion

        #region Public Methods

        public void ViewHitUI()
        {
            AllClean();
            if (PlayerManager.LocalPlayerInstance.tag == "Victim")
            {
                DayUI.SetActive(true);
            }
            else if (PlayerManager.LocalPlayerInstance.tag == "Reaper")
            {
                DayUIReaper.SetActive(true);
            }
        }
        
        public void ViewDayTimeUI()
        {
            AllClean();
            if (PlayerManager.LocalPlayerInstance.tag == "Victim")
            {
                DayUI.SetActive(true);
            }
            else if (PlayerManager.LocalPlayerInstance.tag == "Reaper")
            {
                DayUIReaper.SetActive(true);
            }

            DayMark.SetActive(true);
            NightMark.SetActive(false);
        }

        public void ViewNightTimeUI()
        {
            AllClean();
            if (PlayerManager.LocalPlayerInstance.tag == "Victim")
            {
                NightUI.SetActive(true);
            }
            else if (PlayerManager.LocalPlayerInstance.tag == "Reaper")
            {
                NightUIReaper.SetActive(true);
            }
            NightMark.SetActive(true);
            DayMark.SetActive(false);
        }
        
        /// <summary>
        /// View Game Rule, effect, etc. when Gamestate 'Expain'
        /// in parameter 'sec' time.
        /// ex) sec = 60 --> all UI view in 60 seconds
        /// </summary>
        /// <param name="sec"></param>
        public void ViewExplain(int sec)
        {
            AllClean();
            ExplainUI.SetActive(true);
            ExplainUI.GetComponent<ExplainUI>().ViewExplain(sec);
        }

        public void ViewWinUI(string reason)
        {
            AllClean();
            WinUI.SetActive(true);
            WinUI.GetComponentInChildren<TipeText>().SetText(reason, 5f);
        }

        public void ViewDefeatUI(string reason)
        {
            AllClean();
            DefeatUI.SetActive(true);
            DefeatUI.GetComponentInChildren<TipeText>().SetText(reason, 5f);
        }

        #endregion

        #region Private Methods

        private void AllClean()
        {
            DayUI.SetActive(false);
            DayUIReaper.SetActive(false);
            NightUI.SetActive(false);
            NightUIReaper.SetActive(false);
            ExplainUI.SetActive(false);
            WinUI.SetActive(false);
            DefeatUI.SetActive(false);
        }

        #endregion

        #region CallBacks

        /// <summary>
        /// If Timer callBack this, Callback MafiaManger's OnTimerOver.
        /// Because separate dependency Timer -- MafiaManager
        /// </summary>
        public void OnTimerOver()
        {
            MafiaManager.Instance.OnTimerOver();
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            Instance = this;
            
            if(DayUI == null)
            {
                if (NightUI == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> Day time UI on MafiaManager", this);
                }
            }
            if (NightUI == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Night time UI on MafiaManager", this);
            }
            if (NightUIReaper == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Night time Reaper UI on MafiaManager", this);
            }
            if (ExplainUI == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> ExplainUI on MafiaManager", this);
            }
            if (WinUI == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> WinUI on MafiaManager", this);
            }
            if (DefeatUI == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> DefeatUI on MafiaManager", this);
            }

            GameTimer = GetComponent<Timer>();
            if(GameTimer == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Timer on MafiaManager", this);
            }
        }
        
        #endregion
    }
}