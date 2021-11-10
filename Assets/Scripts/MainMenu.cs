using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Manages main menu
public class MainMenu : MonoBehaviour
{
    // The name of the scene that contains the game itself
    public string sceneToLoad;

    public RectTransform sceneIntroPanel;

    // Represents the scene background loading. This is used to control
    // when the scene should switch over.
    AsyncOperation sceneLoadingOperation;

    // To animate music (fade out)
    public Animator musicAnimator;
    public float waitTime = 1.5f;

    // On Start, begin loading the game
    public void Start() {
        // Make sure scene intro panel is active
        if(sceneIntroPanel) {
            sceneIntroPanel.gameObject.SetActive(true);
        }

        // Begin loading the scene in the background
        sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        // ... but don't actually switch to the new scene until we're ready
        sceneLoadingOperation.allowSceneActivation = false;
    }

    // Called when the New Game button is tapped
    public void LoadScene() {
        // Make the 'Loading' overlay visible
        //loadingOverlay.gameObject.SetActive(true);
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene() {
        
        // Trigger fade out on music and wait a little
        musicAnimator.SetTrigger("fadeOut");
        yield return new WaitForSeconds(waitTime);

        // Tell the scene loading operation to switch scenes when it's done loading
        sceneLoadingOperation.allowSceneActivation = true;
    }
}
