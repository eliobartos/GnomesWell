using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adjust the camera to always match the Y-position of a target object
// with certain limits
public class CammeraFollow : MonoBehaviour
{

    public Transform target;

    // The highest/lowest point the camera can go.
    public float topLimit = 10.0f;
    public float bottomLimit = -10.0f;

    // How quickly we should move towards the target
    public float followSpeed = 0.5f;

    // After all objects have updated position, work out where the camera should be

    void LateUpdate() {
        if (target != null) {

            // Get position of a target
            Vector3 newPosition = this.transform.position;

            newPosition.y = Mathf.Lerp(newPosition.y, target.position.y, followSpeed);

            // Clamp this new location within our limits
            newPosition.y = Mathf.Min(newPosition.y, topLimit);
            newPosition.y = Mathf.Max(newPosition.y, bottomLimit);

            // Update new location
            transform.position = newPosition;
        }
    }

    // For drawing a line from top point to bottom point in unity editor
    // when camera is selected
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;

        Vector3 topPoint = new Vector3(this.transform.position.x, topLimit, this.transform.position.z);
        Vector3 bottomPoint = new Vector3(this.transform.position.x, bottomLimit, this.transform.position.z);

        Gizmos.DrawLine(topPoint, bottomPoint);
    }
}
