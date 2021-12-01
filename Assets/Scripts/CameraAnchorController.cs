using UnityEngine;
using System.Collections;

public class CameraAnchorController : MonoBehaviour
{
    public float xAcceleration = 0;
    public float yAcceleration = 0;
    public float accelerationRate; //set to 1 in inspector
    public Vector2 acceleration;
    public Vector2 velocity;

    float friction = 0.98f;
    public float xMin; //set to -5 in inspector
    public float xMax; //set to 5 in inspector
    public float yMin; //set to -5 in inspector
    public float yMax; //set to 5 in inspector
    //these -5/+d5 are because a Unity plane's origin is in the centre.

    void Update()
    {
        xAcceleration = 0;
        yAcceleration = 0;

        //acceleration
        xAcceleration += Input.GetKey(KeyCode.LeftArrow)    ? -1 : 0;
        xAcceleration += Input.GetKey(KeyCode.RightArrow)   ? +1 : 0;
        yAcceleration += Input.GetKey(KeyCode.UpArrow)      ? +1 : 0;
        yAcceleration += Input.GetKey(KeyCode.DownArrow)    ? -1 : 0;
        acceleration = new Vector2(xAcceleration, yAcceleration); //use Vector2 here or we incur extra, costly sqrt calls in magnitude.
        acceleration = Vector2.ClampMagnitude(acceleration, 1.0f); //normalize it.
        acceleration *= accelerationRate;


        Debug.Log("yAcceleration: " + yAcceleration);


        //speed
        velocity += acceleration;
        velocity *= friction;

        //position
        float xPos = transform.localPosition.x + velocity.x * Time.deltaTime;
        float yPos = transform.localPosition.z + velocity.y * Time.deltaTime; //note the interchanged y/z here due to Unity's coord system.

        //We'd usually just use 2 Mathf.Clamp(val, min, max) calls here, but we need to know
        //the outcome of the clamping, so as to also restrict velocity if we hit a map side.
        if (xPos < xMin)
        {
            xPos = xMin; 
            velocity = new Vector2(0, velocity.y); 
        }
        if (xPos > xMax)
        {
            xPos = xMax; 
            velocity = new Vector2(0, velocity.y);
        }
        if (yPos < yMin)
        {
            yPos = yMin; 
            velocity = new Vector2(velocity.x, 0);
        }
        if (yPos > yMax)
        {
            yPos = yMax; 
            velocity = new Vector2(velocity.x, 0);
        }

        xPos = Mathf.Clamp(xPos, xMin, xMax); //limit to the plane's x     extent in local space
        yPos = Mathf.Clamp(yPos, yMin, yMax); //limit to the plane's y (z) extent in local space

        //update transform
        transform.position = new Vector3(xPos, 0, yPos);
    }
}