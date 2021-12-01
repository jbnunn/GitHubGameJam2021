using UnityEngine;

public class Package : MonoBehaviour
{
    
    public float floatSpeed = 60.0f;
    public float rotationSpeed = 180.0f;
    public float destroyAfterSeconds = 1.5f;
    
    void Update() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.down * Time.deltaTime * floatSpeed, Space.World);
        if (gameObject != null)
            Object.Destroy(gameObject, destroyAfterSeconds);
    }

    void OnColliderEnter(Collider other) {
        if (other != null) 
            Destroy(gameObject);
    }

}
