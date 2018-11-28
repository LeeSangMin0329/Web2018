// # this class only exist when all players setting in scene.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class MafiaManager : Photon.MonoBehaviour
    {
        #region ReadOnly Properties

        [SerializeField]
        private readonly int _dayTimeAmount = 180;
        [SerializeField]
        private readonly int _nightTimeAmount = 30;
        [SerializeField]
        private readonly int _explainTimeAmount = 90;
        [SerializeField]
        private readonly int _resultUITimeAmount = 5;

        #endregion

        #region Public Properties

        // Singleton
        public static MafiaManager Instance;

        [Tooltip("Spawn position when you game start or map out or etc.")]
        public Transform SpawnTransform;

        #endregion

        #region Private Properties

        // FSM
        private enum GameState { Init, Explain, DayTime, NightTime, Result, Exit, Died }
        private GameState _currentState;

        // dead count in one day.
        // why initial value 1?
        // because avoid first day win decision
        private int _oneDayDiedCount;


        // win flag
        private bool _isVictimWin;
        private bool _isReaperWin;

        // This value only use master client
        private int _playerCount;
        private int _playerDeathCount;
        private bool _isReaperDied;


        #endregion

        #region Private Methods

        /// <summary>
        /// select reaper and change tag
        /// respawn all character for game start
        /// </summary>
        private void GameInit()
        {
            // Fix it : PlayerManager _localPlayerManager = PlayerManager.LocalPlayerInstance;
            PlayerManager _localPlayerManager = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>();
            _playerCount = PhotonNetwork.room.PlayerCount;
            _playerDeathCount = 0;
            _isReaperDied = false;

            _isVictimWin = false;
            _isReaperWin = false;

            _oneDayDiedCount = 1;

            Debug.Log("Init Player count : " + _playerCount);

            // Master client Select Role
            if (PhotonNetwork.isMasterClient)
            {
                _localPlayerManager.SetPlayerTag();
                //PhotonNetwork.autoCleanUpPlayerObjects = false;
                PhotonNetwork.room.IsOpen = false;
            }

            GameManager.Instance.IsLevelLoad = false;

            // Respawn all Player Start position 
            _localPlayerManager.ReSpawn(SpawnTransform.position);
        }

        /// <summary>
        /// Control character move and chat when each state.
        /// </summary>
        /// <param name="currentState"></param>
        private void BehaviourControl(GameState currentState)
        {
            if(currentState == GameState.Explain)
            {
                // Character can't move.
                PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().SetStateMoveable(false);
            }
            else if(currentState == GameState.DayTime)
            {
                PlayerManager.LocalPlayerInstance.GetComponent<BroadcastChat>().SetActiveChat(true);
                PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().SetStateMoveable(true);
            }
            else if(currentState == GameState.NightTime)
            {
                // If victim's character can't move.
                if (PlayerManager.LocalPlayerInstance.CompareTag("Victim"))
                {
                    // shut chat. because victim is sleep.
                    PlayerManager.LocalPlayerInstance.GetComponent<BroadcastChat>().SetActiveChat(false);
                    PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().SetStateMoveable(false);
                }
            }
            else if(currentState == GameState.Result || currentState == GameState.Died)
            {
                PlayerManager.LocalPlayerInstance.GetComponent<BroadcastChat>().SetActiveChat(false);
                PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().SetStateMoveable(false);
            }
        }

        /// <summary>
        /// Call when change to day time
        /// </summary>
        private void ChangeDayTime()
        {
            UIManager.Instance.ViewDayTimeUI();
        }

        /// <summary>
        /// Call when change to night time
        /// </summary>
        private void ChangeNightTime()
        {
            UIManager.Instance.ViewNightTimeUI();

            //if (PlayerManager.LocalPlayerInstance.tag == "Reaper")
            {
                photonView.RPC("RequestSceneObjectInstantiate", PhotonTargets.MasterClient, "Item_Knife", PlayerManager.LocalPlayerInstance.transform.position);
            }
        }


        // 1. nobody not dead one day long.
        // 2. 2 player survive. with reaper.
        // 3. 2 player survive. without reaper.
        private int _ending = 0;
        /// <summary>
        /// Call when Changed Daytime
        /// Decision basically only a master client.
        /// [Reaper win]
        /// 1. survive 2 players who reaper and victim.
        /// [Victim Win]
        /// 1. Nobody don't died in one day all.
        /// </summary>
        private void DecideWhoWin()
        {
            Debug.Log("playercount " + _playerCount);
            Debug.Log("_Deathcount " + _playerDeathCount);
            Debug.Log("oneday death " + _oneDayDiedCount);

            // Nobody don't died. so reaper is lose.
            if (_oneDayDiedCount == 0)
            {
                _isVictimWin = true;
                _isReaperWin = false;
                _ending = 1;
            }
            else
            {
                // reset for next day
                _oneDayDiedCount = 0;
            }

            if ((_playerCount - _playerDeathCount) <= 2)
            {
                // If Reaper alive, Reaper win.
                if (!_isReaperDied)
                {
                    // repaer win.
                    _isVictimWin = false;
                    _isReaperWin = true;
                    _ending = 2;
                }
                // Alive 2players without reaper. victim win.
                else
                {
                    _isVictimWin = true;
                    _isReaperWin = false;
                    _ending = 3;
                }
            }

            // Master client decide the win.
            if (PhotonNetwork.isMasterClient)
            {
                if (_isVictimWin || _isReaperWin)
                {
                    SetGameState(GameState.Result);
                }
            }
        }

        private void ViewResultUI()
        {
            if (_isReaperWin)
            {
                Debug.Log("reaper win");
                string _playerTag = PlayerManager.LocalPlayerInstance.tag;
                if (_playerTag == "Reaper")
                {
                    UIManager.Instance.ViewWinUI("당신은 살아남았습니다.");
                }
                else if (_playerTag == "Victim")
                {
                    UIManager.Instance.ViewDefeatUI("살인자를 잡지 못했습니다.");
                }
                else
                {
                    Debug.LogError("<Color=Red><a>Tag Invild</a></Color> SetOtherPlayerState on MafiaManager", this);
                }
            }
            else if (_isVictimWin)
            {
                Debug.Log("victim win");
                string _playerTag = PlayerManager.LocalPlayerInstance.tag;
                if (_playerTag == "Reaper")
                {
                    UIManager.Instance.ViewDefeatUI("아무도 살해하지 못했습니다.");
                }
                else if (_playerTag == "Victim")
                {
                    UIManager.Instance.ViewWinUI("살인자를 잡고 당신은 살아남았습니다.");
                }
                else
                {
                    Debug.LogError("<Color=Red><a>Tag Invild</a></Color> SetOtherPlayerState on MafiaManager", this);
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>GameState.Result Invild</a></Color> SetOtherPlayerState on MafiaManager", this);
            }
        }

        #endregion

        #region RPC Methods

        /// <summary>
        /// change state to parameter
        /// MasterClient only can change state. 
        /// so other player fixed masterclient's timer when state change 
        /// </summary>
        /// <param name="state">want change state</param>
        private void SetGameState(GameState state)
        {
            // only master client can change state order
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("SetOtherGameState", PhotonTargets.All, state);
            }
        }

        /// <summary>
        /// [RPC]
        /// First thing to do when state changed
        /// </summary>
        /// <param name="state"></param>
        [PunRPC]
        private void SetOtherGameState(GameState state)
        {
            // if died, only can change exit state.
            if (_currentState == GameState.Died && state != GameState.Exit)
            {
                return;
            }

            _currentState = state;

            if (_currentState == GameState.Init)
            {

            }
            else if (_currentState == GameState.Explain)
            {
                BehaviourControl(_currentState);
                GameInit();
                UIManager.Instance.GameTimer.SetTimer(_explainTimeAmount);
                UIManager.Instance.ViewExplain(_explainTimeAmount);

                SoundManager.Instance.PlayExplainBGM();
            }
            else if (_currentState == GameState.DayTime)
            {
                BehaviourControl(_currentState);

                ChangeDayTime();
                UIManager.Instance.GameTimer.SetTimer(_dayTimeAmount);

                SoundManager.Instance.PlayDaytimeBGM();

                // Win decision
                DecideWhoWin();
            }
            else if (_currentState == GameState.NightTime)
            {
                BehaviourControl(_currentState);

                ChangeNightTime();
                UIManager.Instance.GameTimer.SetTimer(_nightTimeAmount);

                SoundManager.Instance.PlayNighttimeBGM();
            }
            else if (_currentState == GameState.Result)
            {
                BehaviourControl(_currentState);

                ViewResultUI();
                UIManager.Instance.GameTimer.SetTimer(_resultUITimeAmount);

                SoundManager.Instance.PlayResultBGM();
            }
            else if (_currentState == GameState.Exit)
            {
                if (GameManager.Instance != null)
                {
                    // All user leave loom
                    GameManager.Instance.LeaveRoom();
                }
                else
                {
                    Debug.LogError("MafiaManager.SetOtherGameState : GameManager instance not exist.");
                }
            }
            // Only use state died player in local.
            else if (_currentState == GameState.Died)
            {
                BehaviourControl(_currentState);

                UIManager.Instance.ViewDefeatUI("당신은 살해당했습니다.");
                UIManager.Instance.GameTimer.SetTimer(_resultUITimeAmount);

                SoundManager.Instance.PlayResultBGM();
            }
            else
            {
                Debug.LogError("<Color=Red><a>State Invild</a></Color> OnTimerout on MafiaManager", this);
            }
        }

        /// <summary>
        /// If player died, notice itselt to MafiaManager use this method.
        /// </summary>
        /// <param name="tag">player tag</param>
        public void SomeoneDied(string tag)
        {
            // Only Changed My state.
            SetOtherGameState(GameState.Died);

            photonView.RPC("PlayerDied", PhotonTargets.All, tag);
        }
        /// <summary>
        /// All player manage player dead count.
        /// Because we don't know when master client died.
        /// so if change master client, dicide use to own count.
        /// </summary>
        /// <param name="tag"></param>
        [PunRPC]
        private void PlayerDied(string tag)
        {
            _oneDayDiedCount++;
            _playerDeathCount++;

            if (tag == "Reaper")
            {
                _isReaperDied = true;
            }
        }

        /// <summary>
        /// Create Scene object request master client
        /// </summary>
        /// <param name="prefabName">request prefab name</param>
        /// <param name="position">position to create</param>
        [PunRPC]
        private void RequestSceneObjectInstantiate(string prefabName, Vector3 position)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.InstantiateSceneObject(prefabName, position, Quaternion.identity, 0, null);
            }
        }

        #endregion

        #region CallBacks

        /// <summary>
        /// Callback from UIManager when UIManager.GameTimer time out.
        /// </summary>
        public void OnTimerOver()
        {
            if (_currentState == GameState.Init)
            {
                SetGameState(GameState.Explain);
            }
            else if (_currentState == GameState.Explain)
            {
                SetGameState(GameState.DayTime);
            }
            else if (_currentState == GameState.DayTime)
            {
                SetGameState(GameState.NightTime);
            }
            else if (_currentState == GameState.NightTime)
            {
                SetGameState(GameState.DayTime);
            }
            else if (_currentState == GameState.Result)
            {
                SetGameState(GameState.Exit);
            }
            else if (_currentState == GameState.Exit)
            {

            }
            else if (_currentState == GameState.Died)
            {
                // Only Changed my state.
                SetOtherGameState(GameState.Exit);
            }
            else
            {
                Debug.LogError("<Color=Red><a>State Invild</a></Color> OnTimerout on MafiaManager", this);
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            Instance = this;

            if (SpawnTransform == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Spawn Transform on MafiaManager", this);
            }

            _currentState = GameState.Init;
        }

        #endregion
    }
}
