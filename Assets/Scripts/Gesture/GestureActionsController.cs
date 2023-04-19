using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A mediator between <see cref="GestureDetection"/> and the implementations of
/// <see cref="GestureAction"/>
/// </summary>
[RequireComponent(typeof(GestureDetection))]
[DisallowMultipleComponent()]
public sealed class GestureActionsController : MonoBehaviour
{
    [SerializeField] GestureAction[] actions;    
    GestureDetection detection;

    bool CanGesture => actions != null && actions.Length > 0;

    private void Awake()
    {
        detection = GetComponent<GestureDetection>();
        actions = GetComponentsInChildren<GestureAction>();
        
        if (actions is null || actions.Length is 0)
        {
            Debug.LogWarning($"GestureActionController: no actions found in children. Disabling");
            this.enabled = false;
        }
    }
    
    void OnEnable()
    {
        detection.OnGesturePerformed += OnGesturePerformed;
    }

    private void OnDisable()
    {
        detection.OnGesturePerformed -= OnGesturePerformed;
    }

    private void OnGesturePerformed(Gesture gesture)
    {
        if (CanGesture is false) return;

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].TryPerformGesture(gesture);
        }
    }
}
