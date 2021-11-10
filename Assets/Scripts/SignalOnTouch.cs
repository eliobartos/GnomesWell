using UnityEngine;
using UnityEngine.Events;

// Invokes a Unity Event when the Player collides with this object
[RequireComponent (typeof(Collider2D))]
public class SignalOnTouch : MonoBehaviour
{
    // The UnityEvent to run when we collide
    // Attach methods to run in the editor
    public UnityEvent onTouch;

    // If true, attempt to play an AudioSource when we collide
    public bool playAudioOnTouch = true;

    // When we enter a trigger area, call SendSignal
    void OnTriggerEnter2D(Collider2D collider) {
        SendSignal (collider.gameObject);
    }

    // When we collide with this object, call SendSignal
    void OnCollisionEnter2D(Collision2D collision) {
        SendSignal (collision.gameObject);
    }

    void SendSignal(GameObject objectThatHit) {
        
        // Was this object tagged Player?
        if (objectThatHit.CompareTag("Player")) {
            
            // If we should play a sound, attempt to play it
            if (playAudioOnTouch) {
                var audio = GetComponent<AudioSource>();

                // If we have an audio component, and this component's parents are active then play
                if (audio && audio.gameObject.activeInHierarchy) {
                    audio.Play();
                }

                // Invoke the event
                onTouch.Invoke();
            }
        }
    }
}
