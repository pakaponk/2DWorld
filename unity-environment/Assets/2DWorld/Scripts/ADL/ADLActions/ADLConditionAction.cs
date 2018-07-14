public class ADLConditionAction : ADLAction
{
    /*
     * Total ADLAction within if block
     */
    public int totalActions;

    public ADLConditionAction(string name): base(name) {

    }

    public new bool PerformAction(ADLAgent agent) {
        base.PerformAction(agent);
        return this.GetBoolParameter(0);
    }

    protected override void Perform(ADLAgent agent)
    {
        //Do Nothing
    }
}