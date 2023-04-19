using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Uses player input, the velocity of the controllers and the orientation from 
/// <see cref="HandOrientationDetector"/> to detect gestures. If gestures are detected,
/// raises the <see cref="OnGesturePerformed"/> event.
/// </summary>
[DisallowMultipleComponent()]
public sealed class GestureDetection : MonoBehaviour
{
    const int angleThreshold = 45;
    const int complement = 135;

    public event Action<Gesture> OnGesturePerformed;

    [SerializeField] bool leftHandGesturesEnabled = true;
    [SerializeField] bool rightHandGesturesEnabled = true;

    /// <summary> The transform that orientations are relative to. This should be the player's avatar / forward facing. /// </summary>
    [SerializeField] Transform relativeTo;
    [SerializeField] HandOrientationDetector leftOrientation;
    [SerializeField] HandOrientationDetector rightOrientation;


    [Header("Left Hand Input")]
    [SerializeField] InputActionReference leftSelectAction;
    [SerializeField] InputActionReference leftActivateAction;

    [Header("Right Hand Input")]
    [SerializeField] InputActionReference rightSelectAction;
    [SerializeField] InputActionReference rightActivateAction;

    [Header("Settings")]
    [SerializeField] float lineDistance = 0.3f;


    Transform leftHand, rightHand;
    Detector leftDetector, rightDetector;
    bool leftSelected, rightSelected, leftActivated, rightActivated;

    Gesture gesture;


    private void Start()
    {
        leftHand = leftOrientation.transform;
        rightHand = rightOrientation.transform;

        leftDetector = new Detector(this, leftHand, isLeft: true);
        rightDetector = new Detector(this, rightHand, isLeft: false);

        SubscribeToInputEvents();
    }

    #region Input Event Handling

    void SubscribeToInputEvents()
    {
        leftSelectAction.action.started += ctx => OnStartSelect(true);
        leftSelectAction.action.canceled += ctx => OnCanceledSelect(true);
        leftActivateAction.action.started += ctx => OnStartActivate(true);
        leftActivateAction.action.canceled += ctx => OnCanceledActivate(true);

        rightSelectAction.action.started += ctx => OnStartSelect(false);
        rightSelectAction.action.canceled += ctx => OnCanceledSelect(false);
        rightActivateAction.action.started += ctx => OnStartActivate(false);
        rightActivateAction.action.canceled += ctx => OnCanceledActivate(false);
    }

    private void OnStartSelect(bool isLeft)
    {
        if (isLeft) leftSelected = true;
        else rightSelected = true;
        UpdateDetector(isLeft);
    }

    private void OnCanceledSelect(bool isLeft)
    {
        if (isLeft) leftSelected = false;
        else rightSelected = false;
        UpdateDetector(isLeft);
    }

    void OnStartActivate(bool isLeft)
    {
        if (isLeft) leftActivated = true;
        else rightActivated = true;
        UpdateDetector(isLeft);
    }

    void OnCanceledActivate(bool isLeft)
    {
        if (isLeft) leftActivated = false;
        else rightActivated = false;
        UpdateDetector(isLeft);
    }

    void UpdateDetector(bool isLeft)
    {
        if (isLeft)
        {
            if (leftSelected && leftActivated) leftDetector.StartDetection();
            else if (!leftSelected || !leftActivated) leftDetector.StopDetection();
        }
        else
        {
            if (rightSelected && rightActivated) rightDetector.StartDetection();
            else if (!rightSelected || !rightActivated) rightDetector.StopDetection();
        }
    }

    #endregion

    private void FixedUpdate()
    {
        // If gesturing is enabled and the player is both gripping and pressing the trigger...
        if( leftHandGesturesEnabled && leftActivated && leftSelected )
        {
            // Check for motions. If any are found...
            if (leftDetector.CheckForMotions())
            {
                // Determine what gesture is being made and fire the event for it.
                gesture = new Gesture(leftOrientation.Determine(relativeTo), leftDetector.motonOrientations, HandFlags.Left);
                OnGesturePerformed?.Invoke(gesture);
            }           
        }

        // Repeat for right hand.
        if( rightHandGesturesEnabled && rightActivated && rightSelected )
        {
            if (rightDetector.CheckForMotions())
            {
                gesture = new Gesture(rightOrientation.Determine(relativeTo), rightDetector.motonOrientations, HandFlags.Right);
                OnGesturePerformed?.Invoke(gesture);
            }         
        }
    }


    /// <summary>
    /// Child class of <see cref="GestureDetection"/>
    /// An instance of this class is used for each player hand to detect hand motions
    /// </summary>
    public sealed class Detector
    {
        readonly GestureDetection parent;
        readonly Transform transform;
        readonly bool isLeft;

        public Detector(in GestureDetection parent, in Transform transform, in bool isLeft)
        {
            this.parent = parent;
            this.transform = transform;
            this.isLeft = isLeft;
        }

        Vector3 startPosition = Vector3.zero;
        float distanceMoved = 0;

        /// <summary>
        /// The motions that have been detected so far in this gesture
        /// </summary>
        public List<Orientation> motonOrientations { get; } = new List<Orientation>();

        public void StartDetection() => startPosition = transform.position;
        public void StopDetection() => distanceMoved = 0;

        public bool CheckForMotions()
        {
            this.motonOrientations.Clear();

            distanceMoved = Vector3.Distance(startPosition, transform.position);

            if (distanceMoved > parent.lineDistance)
            {
                Vector3 direction = (transform.position - startPosition).normalized;
                DetermineMotionOrientation(direction);
            }

            return this.motonOrientations.Count > 0;
        }

        void DetermineMotionOrientation(in Vector3 movementDirection)
        {
            Vector3 heading = isLeft ? movementDirection : -movementDirection;

            float angle = Vector3.Angle(heading, parent.relativeTo.right);
            if (angle < angleThreshold) this.motonOrientations.Add(Orientation.Inward);
            else if (angle > complement) this.motonOrientations.Add(Orientation.Outward);

            angle = Vector3.Angle(movementDirection, parent.relativeTo.up);
            if (angle < angleThreshold) this.motonOrientations.Add(Orientation.Upward);
            else if (angle > complement) this.motonOrientations.Add(Orientation.Downward);

            angle = Vector3.Angle(movementDirection, parent.relativeTo.forward);
            if (angle < angleThreshold) this.motonOrientations.Add(Orientation.Forward);
            else if (angle > complement) this.motonOrientations.Add(Orientation.Backward);
        }

    }
}