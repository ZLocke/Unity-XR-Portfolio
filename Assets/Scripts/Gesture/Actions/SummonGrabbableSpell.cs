using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SummonGrabbableSpell : GestureAction
{
    [Header("Summon Objects")]
    [SerializeField] XRGrabInteractable leftSummonable;
    [SerializeField] XRGrabInteractable rightSummonable;

    private void OnEnable()
    {
        leftSummonable.selectExited.AddListener(OnLeftSelectExit);
        rightSummonable.selectExited.AddListener(OnRightSelectExit);
        Clear();        
    }

    private void OnDisable()
    {
        leftSummonable.selectExited.RemoveListener(OnLeftSelectExit);
        rightSummonable.selectExited.RemoveListener(OnRightSelectExit);
        Clear();
    }

    void OnLeftSelectExit(SelectExitEventArgs args)
    {
        leftSummonable.gameObject.SetActive(false);
    }

    void OnRightSelectExit(SelectExitEventArgs args)
    {
        rightSummonable.gameObject.SetActive(false);
    }

    protected override bool CanPerform(Gesture gesture, bool isLeft) => base.CanPerform(gesture, isLeft)
        && HandIsEmpty(isLeft);

    bool HandIsEmpty( bool isLeft )
    {
        if (isLeft) return leftDirect.hasSelection is false;
        else return rightDirect.hasSelection is false;
    }

    protected override void Perform(Gesture gesture)
    {
        bool isLeft = IsLeft(gesture);        
        Summon(isLeft);
    }

    void Summon(bool isLeft )
    {
        if( isLeft)
        {
            leftSummonable.gameObject.SetActive(true);
            leftSummonable.transform.position = leftDirect.transform.position;
            leftSummonable.transform.rotation = leftDirect.transform.rotation;
            leftDirect.interactionManager.SelectEnter(leftDirect as IXRSelectInteractor, leftSummonable);

        }
        else
        {
            rightSummonable.gameObject.SetActive(true);
            rightSummonable.transform.position = rightDirect.transform.position;
            rightSummonable.transform.rotation = rightDirect.transform.rotation;
            rightDirect.interactionManager.SelectEnter(rightDirect as IXRSelectInteractor, rightSummonable);

        }
    }

    protected override void Clear()
    {
        base.Clear();
        leftSummonable.gameObject.SetActive(false);
        rightSummonable.gameObject.SetActive(false);
    }
}
