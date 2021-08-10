using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AurigaAnchors : MonoBehaviour
{
    [SerializeField] private StepsListener controller;
    [SerializeField] private float angleThreshold = 10;
    [SerializeField] private Material anchorMat;
    [SerializeField] private Transform stepsParent;

    Vector3 posA, posB = Vector3.zero;
    // Quaternion rotationA;
    int steps = 0;
    // bool test;
    void Start()
    {
        controller.onStep += OnStepTaken;
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
            var abDir = posB - posA;
            var bcDir = position - posB;
            float angle = Vector3.Angle(abDir, bcDir);
            Debug.Log(angle);

            if (angle >= angleThreshold)
            {
                CreateAnchor(posB);
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
        Debug.Log($"Anchor created = {steps}");

        GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        anchor.transform.position = position;
        anchor.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        anchor.GetComponent<Renderer>().material = anchorMat;
        anchor.transform.GetComponent<Collider>().enabled = false;

    }
}
