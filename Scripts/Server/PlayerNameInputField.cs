using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Static Propertiy
        //Store the PlayerPref Key to avoid typos
        static string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            string defaultName = "";
            InputField _inputField = GetComponent<InputField>();
            if(_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        #endregion

        #region Custom method

        public void SetPlayerName(string value)
        {
            PhotonNetwork.playerName = value + " "; // black must have " " to Update Name

            // Hash map
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}