using System.Collections;
using UnityEngine;

// Uses the input manager to apply sideways forces to an object. Used to make the gnome swing
// side by side.
public class Swinging : MonoBehaviour
{
    // How much should we swing
    public float swingSensitivity = 100.0f;

    // Use FixedUpdate instead of Update, to play better with physics engine
    void FixedUpdate() {

        // If we have no rigidbody (anymore), remove this component
        if (this.GetComponent<Rigidbody2D>() == null) {
            Destroy (this);
            return;
        }

        // Get the tilt amount from the input manager
        float swing = InputManager.instance.sidewaysMotion;

        // Calculate force to apply
        Vector2 force = new Vector2(swing * swingSensitivity, 0);

        // Apply the force
        this.GetComponent<Rigidbody2D>().AddForce(force);


    }
}
