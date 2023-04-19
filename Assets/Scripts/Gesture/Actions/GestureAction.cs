using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Collections;

/// <summary>
/// Base class for <see cref="Gesture"/> based actions.
/// <seealso cref="GestureActionsController"/>
/// </summary>
public abstract class GestureAction : MonoBehaviour
{
    [SerializeField] protected XRDirectInteractor leftDirect;
    [SerializeField] protected XRDirectInteractor rightDirect;

    [Header("Gesture Action")]
    [SerializeField] Gesture[] validGestures;

    [SerializeField, Tooltip("Whether this gesture requires two hand gestures to perform")] protected bool isDualGesture;
    [SerializeField, Tooltip("Valid Window of time between the first gesture being performed and the second")] float dualGestureTimingWindow = 1;
    protected bool leftTrigger, rightTrigger;


    
    protected bool IsLeft(Gesture gesture) => (gesture.handFlags & HandFlags.Left) == gesture.handFlags;

    protected virtual bool CanPerform(Gesture gesture, bool isLeft) =>
        validGestures != null
        && validGestures.Length > 0
        && (!isDualGesture || CanTrigger(isLeft));

    bool CanTrigger(bool isLeft)
    {
        if (isLeft) return !leftTrigger;
        else return !rightTrigger;
    }
    
    /// <summary>
    /// Checks to see if the given gesture matches any of this action's 
    /// valid gestures. If so, attempts to perform the gesture.
    /// </summary>
    /// <param name="gesture"></param>
    public void TryPerformGesture(Gesture gesture)
    {
        bool isLeft = IsLeft(gesture);
        
        if (!CanPerform(gesture, isLeft)) return;
        
        for (int i = 0; i < validGestures.Length; i++)
        {

            if ((gesture.handFlags & validGestures[i].handFlags) != gesture.handFlags) continue;

            // Check if performed has a matching palm Orientation
            if (!validGestures[i].palmOrientations.Intersect(gesture.palmOrientations).Any()) continue;
            
            // Check if performed has a matching motion orientation
            if (!validGestures[i].motionOrientations.Intersect(gesture.motionOrientations).Any()) continue;

            if (isDualGesture)
            {
                if (isLeft) leftTrigger = true;
                else rightTrigger = true;
                StartCoroutine(TriggerLifetime(isLeft));
                if (!leftTrigger || !rightTrigger) continue;
            }
            
            Perform(gesture);
            
            //Debug.Log($"Performing action due to gesture with the following properties: " +
            //    $"\n Palm: {string.Join(", ", gesture.palmOrientations)} " +
            //    $"\n Motion: {string.Join(", ", gesture.motionOrientations)} " +
            //    $"\n IsLeft: {gesture.handFlags}");

            break;
        }
    }   

    /// <summary>
    /// Logic executed when a valid gesture is performed for the action
    /// </summary>
    /// <param name="gesture"></param>
    protected abstract void Perform(Gesture gesture);

    /// <summary>
    /// Logic executed when the action is ended.
    /// </summary>
    protected virtual void Clear()
    {
        leftTrigger = false;
        rightTrigger = false;
    }

    IEnumerator TriggerLifetime(bool isLeft)
    {
        yield return new WaitForSeconds(dualGestureTimingWindow);
        if (isLeft) leftTrigger = false;
        else rightTrigger = false;
    }
}
