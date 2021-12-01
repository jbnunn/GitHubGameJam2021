using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{

    public GameObject target;
    public Vector3 offset;

    void Start()
    {
        if (target.transform.tag == "Van") {
            offset = new Vector3(0, 7, 0);
        }
    }

    void Update()
    {
        transform.position = target.transform.position + offset;    
    }
}
