using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the game state
public class GameManager : Singleton<GameManager> 
{
    // The location where the gnome should appear
    public GameObject startingPoint;

    // The rope object which lowers and raises the gnome
    public Rope rope;

    // The follow script, which will follow the gnome
    public CammeraFollow cameraFollow;

    Gnome currentGnome;

    // The 'current' gnome
    public GameObject gnomePrefab;

    // The UI component that contains the 'restart' and 'resume' buttons
    public RectTransform mainMenu;

    // The UI component that contains the 'up', 'down' and 'menu' buttons
    public RectTransform gameplayMenu;

    // The UI component that contains the 'you win!' screen
    public RectTransform gameOverMenu;

    // The UI component that contains music credits
    public RectTransform creditsMenu;

    // Intro black panel that fades out
    public RectTransform sceneIntroPanel;

    // If true, ignore all damage (but still show damage effects). The get, set make this a property,
    // to make it show up in the list of methods in the Inspector for Unity Events
    public bool gnomeInvincible {get; set;}

    // How long to wait after dying before creating a new gnome
    public float delayAfterDeath = 1.0f;

    // The sound to play when gnome dies
    public AudioClip gnomeDiedSound;

    // The sound to play when the game is won
    public AudioClip gameOverSound;

    void Start() {
        
        // Enable scene fade in transition
        if(sceneIntroPanel) {
            sceneIntroPanel.gameObject.SetActive(true);
        }

        // When the game starts call Reset to set up the gnome
        Reset();
    }

    // Reset the entire game
    public void Reset() {
        // Turn off the menus, turn on the gameplay UI
        if(gameOverMenu) {
            gameOverMenu.gameObject.SetActive(false);
        }

        if(mainMenu) {
            mainMenu.gameObject.SetActive(false);
        }

        if(gameplayMenu) {
            gameplayMenu.gameObject.SetActive(true);
        }

        // Find All Resettable components and tell them to reset
        var resetObjects = FindObjectsOfType<Resettable>();

        foreach (Resettable r in resetObjects) {
            Debug.Log(r.name);
            r.Reset();
        }

        // Make a new gnome
        CreateNewGnome();

        // Up-pause the game
        Time.timeScale = 1.0f;
    }

    void CreateNewGnome() {

        // Remove the current gnome, if there is one
        RemoveGnome();

        // Create a new Gnome object and make it be our current gnome
        GameObject newGnome = (GameObject)Instantiate(gnomePrefab, startingPoint.transform.position, Quaternion.identity);
        
        currentGnome = newGnome.GetComponent<Gnome>();

        // Make the rope visible
        rope.gameObject.SetActive(true);

        // Connect the rope's trailing end to whichever rigidbody the Gnome object wants
        rope.connectedObject = currentGnome.ropeBody;

        // Reset the rope's length to the default
        rope.ResetLength();

        // Tell the cameraFollow to start tracking the new Gnome object
        cameraFollow.target = currentGnome.cameraFollowTarget;
    }

    void RemoveGnome() {
        
        // Don't do anything if the gnome is invincible
        if (gnomeInvincible) 
            return;

            // Hide the rope
            rope.gameObject.SetActive(false);

            // Stop tracking the gnome
            cameraFollow.target = null;

            // If we have a current gnome, make that no longer be the player
            if (currentGnome != null) {
                // This gnome is no longer holding the treasure
                currentGnome.holdingTreasure = false;

                // Mark this object as not the player (so that colliders won't report when the object hits them)
                currentGnome.gameObject.tag = "Untagged";

                // Find everything that is tagged as "Player" and remove that tag
                foreach (Transform child in currentGnome.transform) {
                    child.gameObject.tag = "Untagged";
                }

                currentGnome = null;
            }
    }

    // Kills the gnome
    void KillGnome(Gnome.DamageType damageType) {
        // If we have an audio source, play "gnome died" sound
        var audio = GetComponent<AudioSource>();
        if (audio) {
            audio.PlayOneShot(this.gnomeDiedSound);
        }

        // Show the damage effect
        currentGnome.ShowDamageEffect(damageType);

        // If we're not invincible, reset the gme and make the gnome not be the current player
        if (gnomeInvincible == false) {

            // Tell the gnome that it died
            currentGnome.DestroyGnome(damageType);

            // Remove the Gnome
            RemoveGnome();

            // Reset the game
            StartCoroutine(ResetAfterDelay());
        }
    }

    // Called when gnome dies
    IEnumerator ResetAfterDelay() {
        // Wait for delayAfterDeath seconds, then call Reset
        yield return new WaitForSeconds(delayAfterDeath);
        Reset();
    }

    // Called when the player touches a trap
    public void TrapTouched() {
        KillGnome(Gnome.DamageType.Slicing);
    }

    // Called when the player touches a fire trap
    public void FireTrapTouched() {
        KillGnome(Gnome.DamageType.Burning);
    }

    public void TreasureCollected() {
        // Tell the gnome that it should have the treasure
        currentGnome.holdingTreasure = true;

        // Disable Treasure Collider and particle effect
        GameObject treasure = GameObject.Find("Treasure");
        treasure.GetComponent<Collider2D>().enabled = false;
        
        foreach(ParticleSystem particleSystem in treasure.GetComponentsInChildren<ParticleSystem>()) {
            particleSystem.Stop();
        } 
    }

    // Called when the player touches the exit
    public void ExitReached() {
        // If we have a player and that player is holding a treasure, game over!
        if(currentGnome != null && currentGnome.holdingTreasure == true) {

            // If we have an audio source, play the "game over" sound
            var audio = GetComponent<AudioSource>();
            if (audio) {
                audio.PlayOneShot(this.gameOverSound);
            }

            // Pause the game
            Time.timeScale = 0.0f;

            // Turn off the gameplay menu and turn on the game over screen
            if (gameOverMenu) {
                gameOverMenu.gameObject.SetActive(true);
            }

            if (gameplayMenu) {
                gameplayMenu.gameObject.SetActive(false);
            }
        }
    }

    // Called when the menu button is tapped, and when the resume game button is tapped
    public void SetPaused(bool paused) {
        
        // If we're paused, stop time and enable the menu (disable game overlay)
        if (paused) {
            Time.timeScale = 0.0f;
            creditsMenu.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            gameplayMenu.gameObject.SetActive(false);
        } else {
            // If we're not paused, resume time and disable the menu,
            // enable the gameplay menu
            Time.timeScale = 1.0f;
            creditsMenu.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(false);
            gameplayMenu.gameObject.SetActive(true);
        }
    }

    // Called when the Credits button is tapped in the menu button
    public void ShowCredits() {

        mainMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(true);

    }

    // Called when the restart button is tapped
    public void RestartGame() {

        // Immediately remove the gnome (instead of killing it)
        Destroy(currentGnome.gameObject);
        currentGnome = null;

        // Now reset the game to create a new gnome
        Reset();
    }

}
