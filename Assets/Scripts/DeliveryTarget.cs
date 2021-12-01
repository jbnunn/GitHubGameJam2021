using UnityEngine;
using TrafficSimulation;

public class DeliveryTarget : MonoBehaviour
{
    public GameObject packageDropTarget;
    public Segment segment;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AutonomousVehicle")
        {
            Van van = other.gameObject.GetComponent<Van>();
            if (van.deliveryTargets.Contains(this))
            {
                if (van.packagesToDeliver > 0) {
                    if (!van.isHacked) {
                        StartCoroutine(van.DeliverPackage(packageDropTarget));
                    }
                }
            }
        }
    }
   
}
