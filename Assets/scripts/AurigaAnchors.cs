using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AurigaAnchors : MonoBehaviour
{
    [SerializeField] private StepsListener controller;
    [SerializeField] private float angleThreshold = 10;
    [SerializeField] private float verticalAngleThreshold = 30;
    [SerializeField] private Material anchorMat;
    [SerializeField] private Transform stepsParent;

    Dictionary<Vector3, string> nodesMap;

    Vector3 posA, posB = Vector3.zero;

    // mapA and mapB is last two positions that were added to node
    // We check angles with this too . too detect slight curve paths
    Vector3 mapNodeA, mapNodeB = Vector3.zero;

    // Quaternion rotationA;
    int steps = 0;
    int lastAnchoredStep;
    // bool test;
    void Start()
    {
        controller.onStep += OnStepTaken;
        nodesMap = new Dictionary<Vector3, string>();
    }

    private void OnStepTaken(Vector3 position, Quaternion rotation)
    {
        if (steps < 2)
        {
            if (posA.Equals(Vector3.zero))
            {
                posA = position;
            }
            else 
            {
                posB = position;
            }

            // Must create anchor!
            CreateAnchor(position);
        }
        else
        {
            var anchorAdded = false;
            var abDir = posB - posA;
            var bcDir = position - posB;
            float angle = Vector3.Angle(abDir, bcDir);

            var abDir2d = new Vector2(posB.x, posB.z) - new Vector2(posA.x, posA.z);
            var bcDir2d = new Vector2(position.x, position.z) - new Vector2(posB.x, posB.z);
            float angle2d = Vector2.Angle(abDir2d, bcDir2d);

            if (angle2d >= angleThreshold || angle > verticalAngleThreshold)
            {
                CreateAnchor(posB);
                CreateAnchor(position);
                anchorAdded = true;
                Debug.Log("2 anchors added");
                Debug.Log(angle);
                Debug.DrawRay(posA, abDir * 10, Color.red, 100.0f);
                Debug.DrawRay(posB, bcDir * 10, Color.green, 100.0f);

            }
            else
            {
                abDir = mapNodeB - mapNodeA;
                bcDir = position - mapNodeB;
                if (Vector3.Angle(abDir, bcDir) > angleThreshold)
                {
                    CreateAnchor(position);
                    anchorAdded = true;
                    Debug.Log("1 anchors added");

                }
            }

            // If it's been more than 10 steps since added an anchor, just add one!
            if (!anchorAdded && (steps - lastAnchoredStep > 10))
            {
                CreateAnchor(position);
            }

            // Debug.Log(Vector3.Angle(abDir, bcDir));
            // if (Vector3.Angle(abDir, bcDir) >= angleThreshold && !test)
            // {
            //     Debug.Log(steps);
            //     Debug.Log($"Angle = {Vector3.Angle(abDir, bcDir)}");
            //     Debug.DrawRay(posA, abDir * 10, steps%2 == 0 ? Color.red : Color.blue, 100.0f);
            //     Debug.DrawRay(posB, bcDir * 10, steps%2 == 0 ? Color.green : Color.yellow, 100.0f);
            //     CreateAnchor();
            //     // test = true;
            // }

            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            anchor.transform.position = new Vector3(position.x, position.y - 0.5f, position.z);
            anchor.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            anchor.transform.GetComponent<Collider>().enabled = false;
            anchor.transform.SetParent(stepsParent);

            posA = posB;
            posB = position;
        }

        steps++;

        // Debug.Log($"x: {position.x}, z: {position.z}");
    }

    private void CreateAnchor(Vector3 position)
    {
        // Debug.Log($"Anchor created = {steps}");

        Vector3 newPos = new Vector3(roundTo2(position.x),
                                    roundTo2(position.y),
                                    roundTo2(position.z));
        
        if (!nodesMap.ContainsKey(newPos))
        {
            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            anchor.transform.position = position;
            anchor.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            anchor.GetComponent<Renderer>().material = anchorMat;
            anchor.transform.GetComponent<Collider>().enabled = false;

            nodesMap.Add(newPos, $"{steps}"); // Instead of steps, add hte anchor id
        }

        if (nodesMap.Count == 1) {
            mapNodeA = position;
        } else if (nodesMap.Count == 2) {
            mapNodeB = position;
        } else {
            mapNodeA = mapNodeB;
            mapNodeB = position;
        }

        lastAnchoredStep = steps;
        
    }

    private float roundTo2(float value)
    {
        return Mathf.Round(value * 100.0f) * 0.01f;
    }
}
