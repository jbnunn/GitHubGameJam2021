using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Panning")]
    public float panSpeed = 5.0f;
    public float maxY = 40.0f;
    public float minY = 0.0f;
    
    [Header("Zooming")]
    public float zoom = 100f;
    private float defaultZoom = 60.0f;
    public float minZoomSize = 30.0f;
    public float maxZoomSize = 80.0f;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        Camera.main.orthographicSize = defaultZoom;
    }

    private void Update() {
        //HandlePanningKeys();
        HandleMouseWheel();
    }

    
    private void HandlePanningKeys(){

        // This doesn't work correctly with orthographic camera

        if (Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.Translate(Vector3.left * Time.deltaTime * panSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.Translate(Vector3.right * Time.deltaTime * panSpeed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * panSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.Translate(Vector3.back * Time.deltaTime * panSpeed);
        }
    }

    private void HandleMouseWheel() {

        // Zoom orthographic camera with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize -= scroll * zoom;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoomSize, maxZoomSize);
        }

        gameManager.SetTargetPositionForScore();
        gameManager.SetTargetPositionForPackages();
       
    }
}
