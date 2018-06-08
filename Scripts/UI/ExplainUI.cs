using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Sangmin.Web2018
{
    public class ExplainUI : MonoBehaviour
    {
        #region ReadOnly Properties

        // Total 12 Section in Explain
        public readonly int SectionCount = 12;

        #endregion

        #region Public Properties

        [Tooltip("Explain text object. required TipeText.")]
        public TipeText ExplainText;

        public bool Enabled { get; private set; }

        #endregion

        #region Private Properties

        // View Image object.
        private Image _viewImage;

        // Each section chaneged sprite to explain.
        private Sprite[] _sprites;

        // Each section view time.
        private float[] _viewTimes;

        // Each section view text
        private string[] _texts;

        private float _timer;
        private int _count;

        // All section view On this time.
        [SerializeField]
        private float _totalViewTime;

        #endregion

        #region Public Methods

        /// <summary>
        /// Start view.
        /// All section view end on time parameter.
        /// </summary>
        /// <param name="sec">Total View time.</param>
        public void ViewExplain(int sec)
        {
            _totalViewTime = sec;
            
            for (int i = 0; i < SectionCount; i++)
            {
                _viewTimes[i] = _totalViewTime / SectionCount;
            }

            _count = 0;
            _viewImage.sprite = _sprites[0];
            _timer = _viewTimes[0];
            ExplainText.SetText(_texts[0], _viewTimes[0] * 0.8f);

            Enabled = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Explain texts
        /// </summary>
        private void SetTexts()
        {
            _texts[0] = "환영합니다.\n여러분의 목표는 여기서 살아남아 탈출하는 것입니다.\n낮 시간에는 모두 모여 사이좋게 지내게 됩니다.";
            _texts[1] = "하지만 모두가 이걸 원하는 것은 아닙니다.\n밤이 된 순간 여러분에 섞여 숨어있는 한명의 살인자는 활동을 개시할 것입니다.";
            _texts[2] = "살인자는 자신이 살아남기 위해 밤 시간에 '오직 한명' 다른 사람을 해칠 수 있으며.";
            _texts[3] = "";
            _texts[4] = "여러분은 살아남기 위해 무슨 수를 써서라도 살인자를 찾아내야 할 것입니다.";
            _texts[5] = "만일 '두명이 남을 때까지' 살인자를 막지 못한다면";
            _texts[6] = "살인자는 살아남아 탈출하게 되고";
            _texts[7] = "남은 한명은 마지막 희생양이 되며 '살인자의 승리'로 끝날 것입니다.";
            _texts[8] = "반대로 낮 시간이 되었지만 전날 '아무도 살해당하지 않았을 경우'";
            _texts[9] = "멍청한 살인자를 희생해 '모두가 탈출할 수 있게' 됩니다.";
            _texts[10] = "물론 살인자는 탈출하지 못하게 되겠지만 말이죠.";
            _texts[11] = "룰을 이해하지 못했다면 옆사람에게 물어보도록 하세요.\n그 사람이 당신 편일거란 보장은 없겠지만요.";
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Awake()
        {
            if(ExplainText == null)
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> ExplainText on ExplainUI.", this);
            }

            // Reference to sprite name
            // Path : Assets/Resources/Sprites/Explain
            _sprites = new Sprite[SectionCount];
            for(int i=0; i<SectionCount; i++)
            {
                _sprites[i] = Resources.Load<Sprite>("Sprites/Explain/Explain" + i);
                if(_sprites[i] == null)
                {
                    Debug.LogWarning("<Color=Red><a>Missing</a></Color> Sprite reference on ExplainUI.", this);
                }
            }

            _viewImage = GetComponent<Image>();
            if(_viewImage == null)
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> Image view reference on ExplainUI.", this);
            }

            _viewTimes = new float[SectionCount];
            _texts = new string[SectionCount];
            SetTexts();

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

            if(_timer <= 0)
            {
                _count++;
                if (_count == SectionCount)
                {
                    Enabled = false;
                    SoundManager.Instance.EffectSelect();
                    gameObject.SetActive(false);
                    return;
                }
                
                _viewImage.sprite = _sprites[_count];
                _timer = _viewTimes[_count];

                // Start Tipe text on half section time.
                ExplainText.SetText(_texts[_count], _viewTimes[_count] * 0.5f);

                if(_count == 4 || _count == 7 || _count == 10)
                {
                    SoundManager.Instance.EffectShot();
                }
            }
        }

        #endregion
    }
}