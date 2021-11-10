using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoots an object periodically
// Has two states, rest, prepare. At the end of prepare it shoots an element
public class ShootPeriodically : MonoBehaviour
{

    // Possible List of states
    public enum state {
        Rest,
        Prepare
    }

    // Time spent in each state
    public float restDuration = 5.0f;
    public float prepareDuration = 2.0f;

    // Object to shoot
    public GameObject projectileObject;
    public Transform shootPosition;
    public string shootDirection;
    public float shootSpeed;

    // Different sprites for different states
    public Sprite restStateSprite;
    public Sprite prepareStateSprite;

    // Tracking current state and time in that state
    private state currentState = state.Rest;
    private float timeInState = 0.0f;

    // Audio clip to play when entering prepare state
    public AudioClip prepareAndFireSound;

    // Start is called before the first frame update
    void Start()
    {

        // Set the state to rest and start counter from zero
        currentState = state.Rest;
        timeInState = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeInState += Time.deltaTime;

        if(currentState == state.Rest & timeInState >= restDuration) {
            ChangeStateToPrepare();

        } else if(currentState == state.Prepare & timeInState >= prepareDuration) {
            ChangeStateToRest();
        }
        
    }

    // Changes sprite used for the object that shoots projectiles
    public void ChangeStateToPrepare() {
        Sprite spriteToUse = prepareStateSprite;

        if (spriteToUse != null) {
            GetComponent<SpriteRenderer>().sprite = spriteToUse;
        }

        // If we have an audio source, play the "prepare and fire" sound
        var audio = GetComponent<AudioSource>();
        if (audio) {
            audio.PlayOneShot(this.prepareAndFireSound);
        }
        
        currentState = state.Prepare;
        timeInState = 0;
    }

    // Shoots an object and changes sprite back to rest
    public void ChangeStateToRest() {
        Sprite spriteToUse = restStateSprite;

        if (spriteToUse != null) {
            GetComponent<SpriteRenderer>().sprite = spriteToUse;
        }

        Shoot();

        currentState = state.Rest;
        timeInState = 0;
    }

    public void Shoot() {
        
        // Calculate correct rotation based on direction of shooting
        float startingRotation = 0.0f;

         switch (shootDirection) {
            case "up":
                startingRotation = 90.0f;        
            break;
            case "down":
                startingRotation = 270.0f;        
            break;
            case "left":
                startingRotation = 180.0f;        
            break;
            case "right":
                startingRotation = 0.0f;        
            break;
        }

        // Instantiate the fireball...
        GameObject projectile = (GameObject)Instantiate(projectileObject, shootPosition.position, Quaternion.Euler(0.0f, 0.0f, startingRotation));

        // And make it go in wanted direction and speed
        GoInDirection projectileGoInDirection = projectile.GetComponent<GoInDirection>();
        projectileGoInDirection.direction = shootDirection;
        projectileGoInDirection.speed = shootSpeed;
    }



}
