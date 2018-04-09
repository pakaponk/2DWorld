using UnityEngine;

public class ADLFlipXAction : ADLAction
{
    public ADLFlipXAction(string name): base(name){

    }

    protected override void Perform(ADLAgent agent)
    {
        Rigidbody2D rigidbody2D = agent.GetComponent<Rigidbody2D>();
        agent.horizonDirection = ADLBaseAgent.GetOppositeDirection(agent.horizonDirection);
    }
}