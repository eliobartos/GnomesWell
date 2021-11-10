using System.Collections;
using UnityEngine;

// Translates the accelerometer data into sideways motion info.
public class InputManager : Singleton<InputManager>  {
    
    // How much we're moving: -1 = full left, +1 = full right
    private float _sidewaysMotion = 0.0f;

    // This property is declared as read-only, so that other classes can't change it

    public float sidewaysMotion {
        get {
            return _sidewaysMotion;
        }
    }

    // Every frame, store the tilt
    void Update () {
        Vector3 accel = Input.acceleration;
        
        _sidewaysMotion = accel.x;
    }
}
