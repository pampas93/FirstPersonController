using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsListener : MonoBehaviour
{
    [SerializeField] private float stepThreshold;
    private Vector3 prevPos;

    public Action<Vector3, Quaternion> onStep;

	void Update () 
    {
        Vector3 currPos = transform.position;
        if (Vector3.Distance(prevPos, currPos) >= stepThreshold)
        {
            onStep?.Invoke(currPos, transform.rotation);
            prevPos = currPos;
        }
    }
}
