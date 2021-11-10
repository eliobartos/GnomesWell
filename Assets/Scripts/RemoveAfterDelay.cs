using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Removes and object after a certain delay
public class RemoveAfterDelay : MonoBehaviour
{
    // How many seconds to wait before removing
    public float delay = 1.0f;

    void Start() {
        // Kick of the 'Remove' coroutine
        StartCoroutine("Remove");
    }

    IEnumerator Remove() {
        // Wait delay in seconds and then destroy the game object attached to this object
        yield return new WaitForSeconds(delay);
        Destroy (gameObject);

    }
}
