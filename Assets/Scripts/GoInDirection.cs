using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Go upwards and wiggle while doing that
public class GoInDirection : MonoBehaviour
{
    public string direction = "up";
    public float speed = 1.0f;
    public float wiggleDistance = 0.0f;
    public float wiggleSpeed = 0.0f;

    // Update is called once per frame
    void Update()
    {
        float wigglePosition = Mathf.Sin(Time.time * wiggleSpeed) * wiggleDistance;

        switch (direction) {
            case "up":
                transform.position = transform.position + new Vector3(wigglePosition * Time.deltaTime, speed * Time.deltaTime, 0);        
            break;
            case "down":
                transform.position = transform.position + new Vector3(wigglePosition * Time.deltaTime, -speed * Time.deltaTime, 0);        
            break;
            case "left":
                transform.position = transform.position + new Vector3(-speed * Time.deltaTime, wigglePosition * Time.deltaTime, 0);        
            break;
            case "right":
                transform.position = transform.position + new Vector3(speed * Time.deltaTime, wigglePosition * Time.deltaTime, 0);        
            break;
        }
        
    }
}
