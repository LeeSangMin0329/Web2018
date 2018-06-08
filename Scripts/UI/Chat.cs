using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class Chat : MonoBehaviour
    {
        #region Readonly value

        [Tooltip("One Line per string length to show chatting.")]
        public readonly int LinePerTextLength = 15;

        #endregion

        #region Private Propertiy

        private Text _chatText;
        private InputField _userInputField;

        // Target Character up to head
        private CharacterStatus _target;
        private float _characterHeight = 1f;
        private Transform _targetTransform;
        private Vector3 _targetPosition;
        
        // Disable chat use coroutine
        private IEnumerator _coroutine = null;

        #endregion

        #region Public Propertiy

        [Tooltip("The show text. Text to show to other players.")]
        public GameObject ChatObject;

        [Tooltip("The Input Field. you want to show other players.")]
        public GameObject UserInputFieldObject;

        [Tooltip("Pixel offset from the player target")]
        public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

        [Tooltip("#Critical. Network shared value. if this value is true, show your chat.")]
        [SerializeField]
        public bool IsShowed = false;

        [Tooltip("#Critical. Network shared value. Show message to other players. in my chat.")]
        [SerializeField]
        public string TransferText;
        
        #endregion

        #region Public Methods

        public void SetTarget(CharacterStatus target)
        {
            if (target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayerMakerManager target for SpeakBubbleSyncInputField.SetTarget.", this);
                return;
            }

            _target = target;
            _targetTransform = _target.GetComponent<Transform>();
        }
        
        /// <summary>
        /// Enable/disable Input Field and transfer text if you call this.
        /// </summary>
        public bool InputFieldActivate()
        {
            // If you key down InputField enable state, transfer text will be changed.
            if (UserInputFieldObject.activeInHierarchy)
            {
                TransferText = _userInputField.text;
                _userInputField.text = string.Empty;
                IsShowed = true;
                _userInputField.ActivateInputField();
            }
            
            // If the inputfield disabled/enable, change state reverse.
            UserInputFieldObject.SetActive(!UserInputFieldObject.activeInHierarchy);
            _userInputField.Select();

            // (UserInputFieldObject.activeInHierarchy == false) <- write chatting so send this.
            return !UserInputFieldObject.activeInHierarchy;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Show message chat and use coroutine disable 3 sencond after
        /// </summary>
        /// <param name="message"></param>
        private void ViewChat(string message)
        {
            // If coroutine don't stop, Stop this. and after this method re start
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = DisableWaitCoroutine(3.0f);

            ChatObject.SetActive(true);

            // If message is too long, cut next line length 'LinePerTextLength'
            if (message.Length > LinePerTextLength)
            {
                for (int i = LinePerTextLength; i < message.Length; i += LinePerTextLength)
                {
                    message = message.Insert(i, "\n");
                }
            }
           
            _chatText.text = message;

            StartCoroutine(_coroutine);
        }

        private IEnumerator DisableWaitCoroutine(float time)
        {
            yield return new WaitForSeconds(time);  
            ChatObject.SetActive(false);
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        }

        // Use this for initialization
        void Start()
        {
            if (UserInputFieldObject == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> User InputField Component on Chat", this);
            }
            if (ChatObject == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> chat text Component on Chat", this);
            }

            // find text
            _chatText = ChatObject.GetComponentInChildren<Text>();
            _userInputField = UserInputFieldObject.GetComponent<InputField>();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsShowed)
            {
                ViewChat(TransferText);
                IsShowed = false;
            }

            if (_target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void LateUpdate()
        {
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
