using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    // Rope segment prefab to use
    public GameObject ropeSegmentPrefab;

    // Contains a list of Rope Segment Objects
    List<GameObject> ropeSegments = new List<GameObject>();

    // Are we currently extending or retracting the rope?
    public bool isIncreasing { get; set; }
    public bool isDecreasing { get; set; }

    // The rigidbody object that the end of the rope should be attached to (gnome)
    public Rigidbody2D connectedObject;

    // The maximum length of a rope segment (if we need more, create a new rope segment)
    public float maxRopeSegmentLength = 1.0f;

    // How quickly we pull in/out rope
    public float ropeSpeed = 4.0f;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Cache the line renderer, so we don't have to look it up every frame
        lineRenderer = this.GetComponent<LineRenderer>();

        // Reset the rope, so that we're ready to go
        ResetLength();
    }

    //Remove all rope segments, and create a new one
    public void ResetLength() {
            foreach (GameObject segment in ropeSegments) {
                Destroy(segment);
            }

            ropeSegments = new List<GameObject>();

            isDecreasing = false;
            isIncreasing = false;

            CreateRopeSegment();
    }

    void CreateRopeSegment() {
        // Create the new rope segment
        GameObject segment = (GameObject)Instantiate(ropeSegmentPrefab, this.transform.position, Quaternion.identity);

        // Make the rope segment a child of this object and keep its position
        segment.transform.SetParent(this.transform, true);

        // Get rigidbody from the segment
        Rigidbody2D segmentBody = segment.GetComponent<Rigidbody2D>();
        SpringJoint2D segmentJoint = segment.GetComponent<SpringJoint2D>();

        // Throw error if segment does not have Rigidbody2D or SpringJoint2D component
        if (segmentBody == null || segmentJoint == null) {
            Debug.LogError("Rope segment does not have RigidBody2d or SpringJoint2D");
            return;
        }

        // Add segment to the start of the list of rope segments
        ropeSegments.Insert(0, segment);

        // Connect the new segment to the rope anchor (this object)
        segmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();

        // If first segment, connect it to the gnome
        if (ropeSegments.Count == 1) {
            // Connect the joint of the connected object  to the segment
            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();

            connectedObjectJoint.connectedBody = segmentBody;
            connectedObjectJoint.distance = 0.1f;

            // Set this joint to be at max length at the beginning
            segmentJoint.distance = maxRopeSegmentLength;

        } else {
            // This is an additional rope segment. Connect previous top one with this one

            // Get the second segment
            GameObject nextSegment = ropeSegments[1];

            // Get the joint that we need to attach to
            SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();

            // Make this joint connect to us
            nextSegmentJoint.connectedBody = segmentBody;

            // Make this segment start at a distance of 0 units away from the previous one - it will be extended
            segmentJoint.distance = 0.0f;
        }



    }

    void RemoveRopeSegment() {
        // If we don't have two or more segments, stop.
        if(ropeSegments.Count < 2) {
            return;
        }

        // Get the top segment and the segment under it
        GameObject topSegment = ropeSegments[0];
        GameObject nextSegment = ropeSegments[1];

        // Connect the second segment to rope's anchor
        SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();

        nextSegmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();

        // Remove the top segment and destroy it
        ropeSegments.RemoveAt(0);
        Destroy(topSegment);

    }
    // Every frame, increase or decrease the rope's length if needed
    void Update()
    {
        // Get the top segment and its join
        GameObject topSegment = ropeSegments[0];
        SpringJoint2D topSegmentJoint = topSegment.GetComponent<SpringJoint2D>();

        if (isIncreasing) {
            
            // We're increasing the rope. If it is at max length, add a new segment, otherwise inrease the top rope segment's length

            if (topSegmentJoint.distance >= maxRopeSegmentLength) {
                CreateRopeSegment();
            } else {
                topSegmentJoint.distance += ropeSpeed * Time.deltaTime;
            }
        }

        if (isDecreasing) {
            
            // We're decrasing the rope. If it's near zero length, remove the segment, otherwise decrease the top segments length

            if (topSegmentJoint.distance <= 0.005f) {
                RemoveRopeSegment();
            } else {
                topSegmentJoint.distance -= ropeSpeed * Time.deltaTime;
            }
        }

        if (lineRenderer != null) {
            // How many poins we want to connect with a line
            lineRenderer.positionCount = ropeSegments.Count + 2;

            // Top vertex is always at the rope's location
            lineRenderer.SetPosition(0, this.transform.position);

            // For every rope segment make the line renderer verterx at that position
            for (int i = 0; i < ropeSegments.Count; i++) {
                lineRenderer.SetPosition(i+1, ropeSegments[i].transform.position);
            }

            // Last point is at object's anchor
            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();

            lineRenderer.SetPosition(ropeSegments.Count + 1, connectedObject.transform.TransformPoint(connectedObjectJoint.anchor));
            
        }
    }
}
