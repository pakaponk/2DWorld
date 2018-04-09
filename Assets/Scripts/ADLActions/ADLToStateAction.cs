public class ADLToStateAction : ADLAction
{
    public ADLToStateAction(string name): base(name){

    }

    public string GetStateName(){
        return this.GetStringParameter(0);
    }

    protected override void Perform(ADLAgent agent)
    {
        agent.simulationState.SetCurrentState(agent.agentScript.FindState(this.GetStateName()));
    }
}