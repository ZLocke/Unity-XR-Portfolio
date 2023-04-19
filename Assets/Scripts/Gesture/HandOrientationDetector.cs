using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Determines hand orientation.
/// <seealso cref="GestureDetection"/>
/// </summary>
public class HandOrientationDetector : MonoBehaviour
{
    const int angleThreshold = 45;
    const int complement = 135;

    [SerializeField] bool isLeft;
    
    new Transform transform;
    public readonly List<Orientation> palmOrientations = new List<Orientation>();

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }
    
    public List<Orientation> Determine(in Transform relativeTo)
    {
        palmOrientations.Clear();
        
        float angle = Vector3.Angle(transform.right, relativeTo.right);
        if (angle < angleThreshold) palmOrientations.Add(Orientation.Inward);
        else if (angle > complement) palmOrientations.Add(Orientation.Outward);

        Vector3 facing = isLeft ? transform.right : -transform.right;
        angle = Vector3.Angle(facing, relativeTo.forward);
        if (angle < angleThreshold) palmOrientations.Add(Orientation.Forward);
        else if (angle > complement) palmOrientations.Add(Orientation.Backward);

        angle = Vector3.Angle(facing, relativeTo.up);
        if (angle < angleThreshold) palmOrientations.Add(Orientation.Upward);
        else if (angle > complement) palmOrientations.Add(Orientation.Downward);
       

        return palmOrientations;
    }
}
