using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class SendSignalProgramatically : MonoBehaviour
{
    public enum DamageType {
        Slicing,
        Burning
    }

    public DamageType damageType;

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

                // Find Game manager object and component
                GameObject gameManagerObject = GameObject.Find("GameManager");
                GameManager gameManager = gameManagerObject.GetComponent<GameManager>();

                // Tell the game manager that trap is touched
                switch (damageType) {

                    case DamageType.Burning:
                        
                        if (gameManager != null) {
                            gameManager.FireTrapTouched();
                        }
                        break;

                    case DamageType.Slicing:
                        if (gameManager != null) {
                            gameManager.TrapTouched();
                        }
                        break;
                }
            }
        }
    }
}
