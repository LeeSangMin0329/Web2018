using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class ItemUI : MonoBehaviour
    {
        private Item _target;

        private float _targetHeight = 3f;
        private Transform _targetTransfrom;

        private Vector3 _targetPosition;

        private Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

        public void SetTarget(Item target)
        {
            if(target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Item target for ItemUI.SetTarget.", this);
                return;
            }

            _target = target;
            _targetTransfrom = _target.GetComponent<Transform>();
        }

        private void Awake()
        {
            GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        }

        private void Update()
        {
            // Destroy itself if the target is null, It;s a fail safe when Photon is destroying Instances of a Player over the network.
            if (_target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        void LateUpdate()
        {
            if(_targetTransfrom != null)
            {
                _targetPosition = _targetTransfrom.position;
                _targetPosition.y += _targetHeight;

                transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + ScreenOffset;
            }
        }
    }
}