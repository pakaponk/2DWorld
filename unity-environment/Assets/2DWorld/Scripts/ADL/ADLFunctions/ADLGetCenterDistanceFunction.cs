using UnityEngine;
using System;

public class ADLGetCenterDistanceFunction: ADLFunction {
    public ADLGetCenterDistanceFunction(string name): base(name) {

    }

    private ADLBaseAgent GetAgent(int parameterIndex) {
        string agentName = this.GetStringParameter(parameterIndex);
        return ADLBaseAgent.FindAgent(agentName, ADLAgent.currentUpdatingAgent.transform.parent);
    }

    public override object PerformFunction()
    {
        ADLBaseAgent firstAgent = this.GetAgent(0);
        ADLBaseAgent secondAgent = this.GetAgent(1);

        try{
            float distance;
            if (ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction].ContainsKey(this)) {
                distance = (float) ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction][this];
            } else {
                distance = Math.Abs(firstAgent.GetComponent<Rigidbody2D>().transform.localPosition.x - secondAgent.GetComponent<Rigidbody2D>().transform.localPosition.x);
                ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction].Add(this, distance);
            }
            return distance;
        } catch(NullReferenceException e) {
            Debug.LogError(e.Message);
            return 0;
        }
    }
}