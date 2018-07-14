using UnityEngine;
using System;

public class ADLIsYFunction : ADLFunction{

    public ADLIsYFunction(string name) : base(name){

    }

    private float GetY(){
        return this.GetFloatParameter(0);
    }

    private ADLBaseAgent GetAgent(){
        string agentName = this.GetStringParameter(1);
        return ADLBaseAgent.FindAgent(agentName);
    }

    public override object PerformFunction()
    {
        ADLBaseAgent agent = this.GetAgent();
        float conditionY = (float) Math.Round(this.GetY(), 2);

        float prevY = (float) Math.Round(agent.prevPosition.y, 2);
        Vector2 currentPosition = agent.GetComponent<Rigidbody2D>().position;
        float currY = (float) Math.Round(currentPosition.y, 2);
        
        //Debug.Log("Curr Y: " + currY + " Prev Y: " + prevY + " Condition Y: " + conditionY);
        if ((prevY <= conditionY && currY >= conditionY) || (prevY >= conditionY && currY <= conditionY)) 
        {
            agent.GetComponent<Rigidbody2D>().position = new Vector2(currentPosition.x, conditionY);
            return true;
        }

        return false;
    }
}