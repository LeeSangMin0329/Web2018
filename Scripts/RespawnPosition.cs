using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPosition : MonoBehaviour
{

    public Transform RespawnObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (RespawnObject)
        {
            other.transform.position = RespawnObject.position;
            other.transform.rotation = RespawnObject.rotation;
        }
    }
}
