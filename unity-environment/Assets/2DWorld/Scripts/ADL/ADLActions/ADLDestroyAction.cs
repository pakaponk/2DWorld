using UnityEngine;

public class ADLDestroyAction : ADLAction
{
    public ADLDestroyAction(string name): base(name){

    }

    protected override void Perform(ADLAgent agent)
    {
        Object.Destroy(agent.gameObject);
    }
}