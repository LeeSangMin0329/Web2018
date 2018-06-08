using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class SoundManager : MonoBehaviour
    {
        // Singleton
        public static SoundManager Instance;

        private AudioSource _source;

        private AudioClip _nonGame;

        private AudioClip _explain;
        private AudioClip _daytime;
        private AudioClip _nighttime;
        private AudioClip _result;
        private AudioClip _shot;
        private AudioClip _select;

        public void PlayExplainBGM()
        {
            if (_explain == null)
            {
                _explain = Resources.Load<AudioClip>("Sound/Hood");
                if (_explain == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> explain bgm for SoundManager.", this);
                }
            }

            _source.clip = _explain;
            _source.loop = true;
            _source.Play();
        }

        public void PlayDaytimeBGM()
        {
            if(_daytime == null)
            {
                _daytime = Resources.Load<AudioClip>("Sound/DayTimeBGM");
                if (_daytime == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> Daytime bgm for SoundManager.", this);
                }
            }
            
            _source.clip = _daytime;
            _source.loop = true;
            _source.Play();
        }

        public void PlayNonGameBGM()
        {
            if(_nonGame == null)
            {
                _nonGame = Resources.Load<AudioClip>("Sound/BGM");
                if(_nonGame == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> nonGame bgm for SoundManager.", this);
                }
            }

            _source.clip = _nonGame;
            _source.loop = true;
            _source.volume = 0.7f;
            _source.Play();
        }

        public void PlayNighttimeBGM()
        {
            if (_nighttime == null)
            {
                _nighttime = Resources.Load<AudioClip>("Sound/NightTimeBGM");
                if (_nighttime == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> Nighttime bgm for SoundManager.", this);
                }
            }

            _source.clip = _nighttime;
            _source.loop = true;
            _source.volume = 0.7f;
            _source.Play();
        }

        public void PlayResultBGM()
        {
            if (_result == null)
            {
                _result = Resources.Load<AudioClip>("Sound/Result");
                if (_result == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> result bgm for SoundManager.", this);
                }
            }

            _source.clip = _result;
            _source.loop = true;
            _source.volume = 0.7f;
            _source.Play();
        }

        public void EffectShot()
        {
            if(_shot == null)
            {
                _shot = Resources.Load<AudioClip>("Sound/CheapShot");
            }

            _source.PlayOneShot(_shot);
        }

        public void EffectSelect()
        {
            if (_select == null)
            {
                _select = Resources.Load<AudioClip>("Sound/Select");
            }

            _source.PlayOneShot(_select);
        }

        // Use this for initialization
        void Start()
        {
            Instance = this;

            _source = gameObject.AddComponent<AudioSource>();
            PlayNonGameBGM();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}