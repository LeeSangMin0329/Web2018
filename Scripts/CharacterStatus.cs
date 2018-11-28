
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class CharacterStatus : Photon.MonoBehaviour, IPunObservable
    {
        #region ReadOnly Properties

        // If damage amount over this value, blood decal instantiate.
        private float _overDamageAmount = 9;
        private Vector3 _decalOffset = new Vector3(0f, 0.1f, 0f);

        #endregion

        #region Public Properties

        public float HP { get; private set; }
        public float MaxHP { get; private set; }

        // If you equiped weapon, value is true
        public bool IsEquiped { get; private set; }

        // Effect
        [Tooltip("Blood Decal. If you damaged HP, Instantiate this object.")]
        public GameObject BloodDecal;

        [Tooltip("In own hand eqiuped weapon 'knife' object")]
        public GameObject EquipedKnife;

        #endregion

        #region Private Properties

        private Transform _ownTransform;

        private float _deltaHP;

        // weapon ID for active/unactive weapon
        private int _equipedWeaponID;

        // Sound
        private AudioSource _audioSource;
        private AudioClip _damagedSound;

        #endregion

        #region Public Methods

        /// <summary>
        /// Damage method
        /// I'm damaged. hp down
        /// </summary>
        /// <param name="amount"></param>
        public void Damage(float amount, Vector3 direction)
        {
            HP -= amount;
            if (HP < 0)
            {
                HP = 0;
            }

            if (HP <= 0)
            {
                return;
            }

            // damage feed back
            direction.y = 0;
            GetComponent<Rigidbody>().AddForce(-direction.normalized * 50f, ForceMode.Impulse);

            // camera shake
            GetComponent<CameraFollow>().ShakeOn(1f);

            // night time use it.
            UIManager.Instance.ViewHitUI();
            PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().SetStateMoveable(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Blood decal Instantiate to player's position.
        /// </summary>
        private void InstantiateBlood()
        {
            if (_deltaHP - HP > _overDamageAmount)
            {
                PlayDamagedSound();
                Instantiate(BloodDecal, _ownTransform.position + _decalOffset, _ownTransform.rotation);
            }
            _deltaHP = HP;
        }

        /// <summary>
        /// Damaged sound shot
        /// use lazy instantiate.
        /// </summary>
        private void PlayDamagedSound()
        {
            if (_damagedSound == null)
            {
                _damagedSound = Resources.Load<AudioClip>("Sound/CheapShot");
                if (_damagedSound == null)
                {
                    Debug.LogWarning("<Color=Red><a>Missing</a></Color> Damaged sound clip reference on CharacterStatus.", this);
                }
            }

            _audioSource.PlayOneShot(_damagedSound);
        }

        #endregion

        #region RPC Methods

        /// <summary>
        /// PlayerHitArea call this.
        /// On Trigger collision. call I'm get this item
        /// so view Id player get item id to all player broadcast
        /// </summary>
        /// <param name="viewID">want get player's view id</param>
        /// <param name="itemID">Item id</param>
        [PunRPC]
        public void GetItem(int viewID, int itemID)
        {
            if (viewID != photonView.viewID)
            {
                return;
            }
            // ID 0 is knife
            if (itemID == 0)
            {
                IsEquiped = true;
                _equipedWeaponID = 0;
                if (gameObject.CompareTag("Reaper") || gameObject.CompareTag("Victim"))
                {
                    EquipedKnife.tag = "MyWeapon";
                }
                EquipedKnife.SetActive(true);
            }
        }

        /// <summary>
        /// To broadcast all player.
        /// view id player Unequip and drop weapon.
        /// </summary>
        /// <param name="viewID">want un equip player's view id</param>
        [PunRPC]
        public void UnEquip(int viewID)
        {

            if (viewID != photonView.viewID)
            {
                return;
            }

            IsEquiped = false;

            if (_equipedWeaponID == 0)
            {
                EquipedKnife.SetActive(false);

                // call Instantiate must one time.
                if (photonView.isMine)
                {
                    photonView.RPC("RequestSceneObjectInstantiate", PhotonTargets.MasterClient, "Item_" + EquipedKnife.name, _ownTransform.position);
                }
            }
        }

        [PunRPC]
        private void RequestSceneObjectInstantiate(string prefabName, Vector3 position)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.InstantiateSceneObject(prefabName, position, Quaternion.identity, 0, null);
            }
        }

        #endregion

        #region Photon.MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            _ownTransform = GetComponent<Transform>();
            if (_ownTransform == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Player Transform on CharacterStatus", this);
            }
            if (BloodDecal == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Blood decal on CharacterStatus", this);
            }
            if (EquipedKnife == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> equiped knife on CharacterStatus", this);
            }

            HP = 100f; // < - reqired re think
            _deltaHP = HP;
            MaxHP = HP;

            IsEquiped = false;

            // Sound
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!enabled)
            {
                return;
            }

            // Blood Effect Instantiate if you over damage.
            // blood effect Seperated from RPC so call this line.
            InstantiateBlood();

            if (!photonView.isMine)
            {
                return;
            }
            // character dead
            if (HP <= 0f)
            {
                CharacterAnimation _ca = GetComponent<CharacterAnimation>();
                if (_ca)
                {
                    _ca.SetDeath();
                }
                else
                {
                    Debug.Log("CharacterAnimation is missing. on CharacterStatus. Is this fine?", this);
                }

                MafiaManager.Instance.SomeoneDied(PlayerManager.LocalPlayerInstance.tag);

                // This way just only one time call
                enabled = false;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(HP);
            }
            else
            {
                HP = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}
