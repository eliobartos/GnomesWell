using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    // The object that the camera should follow
    public Transform cameraFollowTarget;

    public Rigidbody2D ropeBody;

    public Sprite armHoldingEmpty;
    public Sprite armHoldingTreasure;

    public SpriteRenderer holdingArm;

    public GameObject deathPrefab;
    public GameObject flameDeathPrefab;
    public GameObject ghostPrefab;

    public float delayBeforeRemoving = 3.0f;
    public float delayBeforeReleasingGhost = 0.25f;

    public GameObject bloodFountainPrefab;
    public GameObject etherealTrailPrefab;

    bool dead = false;

    bool _holdingTreasure = false;
    
    public bool holdingTreasure {
        get {
            return _holdingTreasure;
        }
        set {
            if(dead == true) {
                return;
            }

            _holdingTreasure = value;

            if (holdingArm != null) {
                if (_holdingTreasure) {
                    holdingArm.sprite = armHoldingTreasure;
                } else {
                    holdingArm.sprite = armHoldingEmpty;
                }
            }
        }
    }

    public enum DamageType {
        Slicing,
        Burning
    }

    public void ShowDamageEffect(DamageType type) {
        switch (type) {
            case DamageType.Burning:
                if(flameDeathPrefab != null) {
                    Instantiate(flameDeathPrefab, cameraFollowTarget.position, cameraFollowTarget.rotation);
                }
                break;

            case DamageType.Slicing:
                if(deathPrefab != null) {
                    Instantiate(deathPrefab, cameraFollowTarget.position, cameraFollowTarget.rotation);
                }
                break;
        }
    }

    public void DestroyGnome(DamageType type) {
        
        // Save the body position to release the ghost
        Transform bodyTransform = transform.Find("Prototype Body");
        Vector3 deathPosition = new Vector3 (bodyTransform.position.x, bodyTransform.position.y, bodyTransform.position.z);

        holdingTreasure = false;

        dead = true;
        
        // find all child objects and randomly disconnect their joints
        foreach (BodyPart part in GetComponentsInChildren<BodyPart>()) {
            
            // Change tags of all parts, and collider children objects "Untagged" to that they do not collect gold when they fall down
            part.tag = "Untagged";
            foreach (Collider2D collider in part.gameObject.GetComponentsInChildren<Collider2D>()) {
                collider.tag = "Untagged";
            }

            switch (type) {
                case DamageType.Burning:
                    // 1 in 3 chance of burning
                    bool shouldBurn = Random.Range(0, 0) == 0;
                    if (shouldBurn) {
                        part.ApplyDamageSprite(type);
                    }
                    break;

                case DamageType.Slicing:
                    // Slice damage always applies a damage sprite
                    part.ApplyDamageSprite(type);
                    break;

            }

            // 1 in 3 change of separating from body
            bool shouldDetach = Random.Range(0, 2) == 0;

            if(shouldDetach) {

                // Make this object remove its rigidbody and collider after it comes to rest
                part.Detach();

                // If we're separating and the damage type was slicing, add a blood fountain
                if (type == DamageType.Slicing) {
                    if(part.bloodFountainOrigin != null && bloodFountainPrefab != null) {
                        // Attach a blood fountain for this detached part
                        GameObject fountain = Instantiate(bloodFountainPrefab, part.bloodFountainOrigin.position, part.bloodFountainOrigin.rotation) as GameObject;

                        fountain.transform.SetParent(this.cameraFollowTarget, false);
                    }
                }
                
                // Disconnect this object
                var allJoints = part.GetComponentsInChildren<Joint2D>();
                foreach (Joint2D joint in allJoints) {
                    Destroy (joint);
                }
            }
        }

        var remove = gameObject.AddComponent<RemoveAfterDelay>();
        remove.delay = delayBeforeRemoving;

        StartCoroutine(ReleaseGhost(deathPosition));
    }

    IEnumerator ReleaseGhost(Vector3 deathPosition) {
        // No ghost prefab? Bail out.
        if (ghostPrefab == null) {
            yield break;
        }     

        // Wait for delayBeforeReleasingGhost seconds
        yield return new WaitForSeconds(delayBeforeReleasingGhost);

        // Add the ghost
        GameObject ghost = (GameObject)Instantiate(ghostPrefab, deathPosition, Quaternion.identity); 

        // Locate Trail origin 
        GameObject etherealTrailOrigin = ghost.transform.GetChild(0).gameObject;

        // Add the trail with the position of new origin
        GameObject etherealTrail = Instantiate(etherealTrailPrefab, etherealTrailOrigin.transform.position , Quaternion.identity);
        etherealTrail.transform.SetParent(ghost.transform, true);
    }
}
