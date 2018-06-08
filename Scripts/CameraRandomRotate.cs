using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRandomRotate : MonoBehaviour
{

    private Transform _ownCamera;
    private float range = 0.5f;

    // Use this for initialization
    void Start()
    {
        _ownCamera = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        Vector3 direction = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));

        Quaternion ro = Quaternion.LookRotation(direction.normalized);
        _ownCamera.rotation = Quaternion.Slerp(_ownCamera.rotation, ro, Time.deltaTime * 0.01f);

    }
}
