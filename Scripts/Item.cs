using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class Item : Photon.MonoBehaviour
    {
        #region Public Properties

        [Tooltip("Item Explain UI. May Instantiate Item object create this too.")]
        public GameObject ItemUIPrefab;

        [Tooltip("#Critical Use RPC parameter. Item ID. default id is 0, you must setting in each prefab")]
        public int ID = 0;

        [Tooltip("Item name. defalut name is 'unknown' you must setting in each prefab.")]
        public string Name = "UNKNOWN";

        [Tooltip("Item Explain. default explain is 'unknown' you must setting in each prefab.")]
        public string Explain = "UNKNOWN";

        #endregion

        #region Private Properties

        private GameObject _uiReference;

        #endregion

        #region Public Methods

        public void PickItem()
        {
            // someone pick item so Destroy object
            photonView.RPC("DestroyItem", PhotonTargets.MasterClient);
        }

        #endregion

        #region RPC Methods

        /// <summary>
        /// Destroy this item(Scene object) request for master client
        /// </summary>
        [PunRPC]
        private void DestroyItem()
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        #endregion

        #region Photon.MonoBehaviour CallBacks

        private void Start()
        {
            // Create the UI
            if (ItemUIPrefab != null)
            {
                _uiReference = Instantiate(ItemUIPrefab) as GameObject;
                _uiReference.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> ItemUiPrefab reference on Item.", this);
            }
            _uiReference.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Reaper") || other.CompareTag("Victim"))
            {
                // If Local player collision, show item UI.
                _uiReference.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Reaper") || other.CompareTag("Victim"))
            {
                _uiReference.SetActive(false);
            }
        }

        #endregion
    }
}