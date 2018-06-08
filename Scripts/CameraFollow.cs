using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : Photon.PunBehaviour
{

    public bool followOnStart = false;

    private bool _isFollowing = false;

    private float _dist = 5.0f;
    private float _height = 7.0f;

    private Transform _own;
    private Transform _cameraTransform;

    private float _shakePower = 2f;
    private bool _shake = false;
    private Vector3 _cameraPos;
    private Vector3 _shakedCameraPos;

    public void ShakeOn(float power)
    {
        _shake = true;
        _shakePower = power;
    }

    #region Photon.PunBehaviour CallBacks

    // Use this for initialization
    void Start ()
    {
        _own = GetComponent<Transform>();

        if (followOnStart)
        {
            OnStartFollowing();
        }
	}

    // Update is called once per frame
    void LateUpdate ()
    {

        // The transform target may not destroy on level load,
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens.
        if(_cameraTransform == null && _isFollowing)
        {
            OnStartFollowing();
        }

         if(_isFollowing)
        {
            // quarter view 
            _cameraTransform.position = _own.position - Vector3.forward * _dist + Vector3.up * _height;
            _cameraTransform.LookAt(_own);
        }

        // camera shake
        if (_shake)
        {
            _cameraPos = _cameraTransform.position;
            if(_shakePower > 0.0f)
            {
                _shakePower -= 5.0f * Time.deltaTime;
            }
            else
            {
                _shake = false;
                _shakePower = 0.0f;
            }
            _shakedCameraPos = UnityEngine.Random.insideUnitCircle * _shakePower;
            _cameraPos.y += _shakedCameraPos.x;
            _cameraPos.z += _shakedCameraPos.y;
            _cameraTransform.position = _cameraPos;
        }
	}

    #endregion

    public void OnStartFollowing()
    {
        _cameraTransform = Camera.main.transform;
        _isFollowing = true;
    }
}
