using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class PlayerCountText : MonoBehaviour
    {
        private Text _text;

        // Use this for initialization
        void Start()
        {
            _text = GetComponent<Text>();
            _text.text = "현재 플레이어 수 : " + PhotonNetwork.room.PlayerCount;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}