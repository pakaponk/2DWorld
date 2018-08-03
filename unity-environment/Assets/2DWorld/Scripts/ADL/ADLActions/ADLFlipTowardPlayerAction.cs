using System;
using UnityEngine;

public class ADLFlipTowardPlayerAction : ADLAction
{
    public ADLFlipTowardPlayerAction(string name): base(name){

    }

    protected override void Perform(ADLAgent agent)
    {
        ADLBaseAgent player = ADLBaseAgent.FindAgent("Player", agent.transform.parent);
        try {
            if (player.transform.localPosition.x > agent.transform.localPosition.x) {
                agent.horizonDirection = ADLBaseAgent.Direction.Normal;
            } else {
                agent.horizonDirection = ADLBaseAgent.Direction.Inverse;
            }
        } catch (Exception e) when (e is NullReferenceException || e is MissingReferenceException) {
            Debug.LogError("Player Not Found: " + e.Message);
        }
    }
}