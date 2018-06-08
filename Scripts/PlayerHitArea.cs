using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class PlayerHitArea : Photon.MonoBehaviour
    {
        #region Public Properties
        
        [Tooltip("#Critical. Network shared variable, order pick onTriggerStayed weapon.")]
        public bool Pick = false;

        [Tooltip("#Critical. Network shared variable, order drop weapon if you have that.")]
        public bool Drop = false;

        #endregion

        #region Private Properties

        // linked character status if you damaged
        private CharacterStatus _characterStatus;

        // parent's photonView
        private PhotonView _photonView;
        
        #endregion
        
        #region Photon.MonoBehavour CallBacks

        // Use this for initialization
        void Start()
        {
            _characterStatus = GetComponentInParent<CharacterStatus>();
            if (_characterStatus == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CharacterStatus on PlayerHitArea", this);
            }

            _photonView = GetComponentInParent<PhotonView>();
            if (_photonView == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PhotonView in parent on PlayerHitArea", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_photonView.isMine)
            {
                return;
            }
            // drop weapon when you equip some weapon.
            if (Drop && _characterStatus.IsEquiped == true)
            {
                _photonView.RPC("UnEquip", PhotonTargets.All, PlayerManager.LocalPlayerInstance.GetPhotonView().viewID);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!(other.CompareTag("Weapon") || other.CompareTag("Item")))
            {
                return;
            }
            if (!_photonView.isMine)
            {
                return;
            }

            // after work  : first damage , big blood
            if (other.CompareTag("Weapon"))
            {
                _characterStatus.Damage(other.GetComponent<Weapon>().DamegeAmount, other.transform.position);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!(other.CompareTag("Weapon") || other.CompareTag("Item")))
            {
                return;
            }
            if (!_photonView.isMine)
            {
                return;
            }

            if (other.CompareTag("Item"))
            {
                // Pick the weapon when your hand empty.
                if (Pick && !_characterStatus.IsEquiped)
                {
                    // try pick item
                    Item item = other.GetComponentInParent<Item>();
                    if(item == null)
                    {
                        Debug.LogError("Check GetComponent where Item script exist.");
                    }

                    // Destroy object on ground item
                    item.PickItem();

                    // CharacterStatus.GetItem broadcast I'm get this item so update you client
                    _photonView.RPC("GetItem", PhotonTargets.All, PlayerManager.LocalPlayerInstance.GetPhotonView().viewID, item.ID);
                }
            }
            else
            {
                //_characterStatus.Damage(other.GetComponent<Weapon>().DamegeAmount * Time.deltaTime);
            }
        }

        #endregion
    }
}