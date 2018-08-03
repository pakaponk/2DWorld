using UnityEngine;
using System;

public class ADLIsXFunction : ADLFunction{
    public ADLIsXFunction(string name) : base(name){

    }

    private float GetX(){
        return this.GetFloatParameter(0);
    }

    private ADLBaseAgent GetAgent(){
        string agentName = this.GetStringParameter(1);
        return ADLBaseAgent.FindAgent(agentName, ADLAgent.currentUpdatingAgent.transform.parent);
    }

    public override object PerformFunction()
    {
        ADLBaseAgent agent = this.GetAgent();

        float conditionX = (float) Math.Round(this.GetX(), 2);

        float prevX = (float) Math.Round(agent.prevPosition.x, 2);
        Vector2 currentPosition = agent.GetComponent<Rigidbody2D>().position;
        float currX = (float) Math.Round(currentPosition.x, 2);

        if ((prevX <= conditionX && currX >= conditionX) || (prevX >= conditionX && currX <= conditionX))
        {
            agent.GetComponent<Rigidbody2D>().position = new Vector2(conditionX, currentPosition.y);
            return true;
        }

        return false;
    }
}