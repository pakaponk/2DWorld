using UnityEngine;

public class ADLGetYFunction: ADLFunction {
    public ADLGetYFunction(string name): base(name) {

    }

    private ADLBaseAgent GetAgent() {
        string agentName = this.GetStringParameter(0);
        return ADLBaseAgent.FindAgent(agentName, ADLAgent.currentUpdatingAgent.transform.parent);
    }

    public override object PerformFunction()
    {
        ADLBaseAgent agent = this.GetAgent();

        string propertyKey = agent.agentName + "Y";
        
        float y;
        if (ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction].ContainsKey(this)) {
            y = (float) ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction][this];
        } else {
            y = agent.GetComponent<Rigidbody2D>().position.y;
            ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[ADLAction.performingAction].Add(this, y);
        }
        return y;
    }
}