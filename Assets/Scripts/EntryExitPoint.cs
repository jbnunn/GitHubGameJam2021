using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryExitPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AutonomousVehicle")
        {
            Destroy(other.gameObject);
        }
    }   
}
