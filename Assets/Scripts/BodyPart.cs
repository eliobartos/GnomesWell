using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class BodyPart : MonoBehaviour
{
    // The sprite to use when ApplyDamageSprite is called with damage type 'slicing'
    public Sprite detachedSprite;

    // The sprite to use when ApplyDamageSprite is called with damage type 'burning'
    public Sprite burnedSprite;

    // Position and rotation of a blood fountain that will appear on the main body
    public Transform bloodFountainOrigin;

    // If true, this object will remove its collision, joints and rigidbody when it comes to rest
    bool detached = false;

    // Decouple this object from the parent, and flag it as needing physics removal
    public void Detach() {
        detached = true;

        this.tag = "Untagged";
    }

    // Every frame, if we're detached, remove physics if the rigidbody is sleeping.
    // This means this detached body part will never get in the way of the gnome.
    public void Update() {

        // If we're not detached, do nothing
        if (detached == false) {
            return;
        }

        // Is our rigidbody sleeping?
        var rigidbody = GetComponent<Rigidbody2D>();

        if (rigidbody.IsSleeping()) {

            // If so, destroy all joins
            foreach (Joint2D joint in GetComponentsInChildren<Joint2D>()) {
                Destroy(joint);
            }

            // ... rigidbodies ...
            foreach (Rigidbody2D body in GetComponentsInChildren<Rigidbody2D>()) {
                Destroy (body);
            }

            // ... and the collider
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>()) {
                Destroy (collider);
            }

            // Remove this script
            Destroy (this);
        }
    }

    public void ApplyDamageSprite(Gnome.DamageType damageType) {
        
        Sprite spriteToUse = null;

        switch (damageType) {
            case Gnome.DamageType.Burning: 
                spriteToUse = burnedSprite;
                break;

            case Gnome.DamageType.Slicing:
                spriteToUse = detachedSprite;
                break;
        }

        if (spriteToUse != null) {
            GetComponent<SpriteRenderer>().sprite = spriteToUse;
        }
    }
}
