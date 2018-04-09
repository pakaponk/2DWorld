using UnityEngine;

public class ADLFlipYAction : ADLAction
{
    public ADLFlipYAction(string name): base(name){

    }

    protected override void Perform(ADLAgent agent)
    {
        Rigidbody2D rigidbody2D = agent.GetComponent<Rigidbody2D>();
        agent.verticalDirection = ADLBaseAgent.GetOppositeDirection(agent.verticalDirection);
    }
}