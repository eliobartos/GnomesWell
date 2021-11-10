using System.Collections;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
where T : MonoBehaviour {
    // The single instance of this class
    private static T _instance;

    public static T instance {
        get {
            // If we haven't already set up instance
            if(_instance == null) {
                // Try to find the object
                _instance = FindObjectOfType<T>();

                // Log if we can't find it
                if(_instance == null) {
                    Debug.LogError("Can't find " + typeof(T) + "!");
                }
            }

            // return the instance so that it can be used
            return _instance;
        }
    }    
}
