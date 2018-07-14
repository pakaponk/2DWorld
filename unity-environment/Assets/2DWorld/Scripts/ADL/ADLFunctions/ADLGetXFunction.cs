using UnityEngine;
using System;

public class ADLGetXFunction: ADLFunction {
    public ADLGetXFunction(string name): base(name) {

    }

    private ADLBaseAgent GetAgent() {
        string agentName = this.GetStringParameter(0);
        return ADLBaseAgent.FindAgent(agentName);
    }

    public override object PerformFunction()
    {
        ADLBaseAgent agent = this.GetAgent();
        
        float x;
        if (ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction].ContainsKey(this)) {
            x = (float) ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction][this];
        } else {
            try{
                x = agent.GetComponent<Rigidbody2D>().position.x;
            }
            catch (Exception e) when (e is NullReferenceException || e is MissingReferenceException){
                Debug.LogWarningFormat("Specified Agent Not Found - {0}", e.Message);
                x = 0;
            }
            ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction].Add(this, x);
        }
        return x;
    }
}