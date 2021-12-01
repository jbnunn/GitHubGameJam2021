using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=7vFwTt4isDY
public class MaskObject : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().material.renderQueue = 3002;        
    }

}
