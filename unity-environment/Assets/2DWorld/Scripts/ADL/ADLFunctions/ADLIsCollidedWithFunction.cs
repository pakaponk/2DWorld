using System;

public class ADLIsCollidedWithFunction : ADLFunction
{
    public ADLIsCollidedWithFunction(String name): base(name) {

    }

    private ADLBaseAgent GetAgent(){
        string agentName = this.GetStringParameter(0);
        return ADLBaseAgent.FindAgent(agentName, ADLAgent.currentUpdatingAgent.transform.parent);
    }

    public override object PerformFunction()
    {
        ADLBaseAgent targetAgent = this.GetAgent();
        ADLBaseAgent self = ADLAgent.currentUpdatingAgent;

        if (self.collisionList.Contains(targetAgent)) {
            return true;
        } else {
            return false;
        }
    }
}