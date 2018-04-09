public class ADLTimesFunction : ADLFunction{

    private int count = 0;

    public ADLTimesFunction(string name) : base(name)
	{

	}

    private int GetAmount(){
        return this.GetIntParameter(0);
    }

    private bool GetIsConditionMatched(){
        return this.GetBoolParameter(1);
    }

    public override object PerformFunction(){ 
        ADLAction currentAction = ADLAction.performingAction;
        ADLAgent currentAgent = ADLAgent.currentUpdatingAgent;

        if (!currentAgent.simulationState.singleQueryProperties[currentAction].ContainsKey(this)) {
            currentAgent.simulationState.singleQueryProperties[currentAction].Add(this, 0);
        }

        int count = (int) currentAgent.simulationState.singleQueryProperties[currentAction][this];
        if (this.GetIsConditionMatched())
        {
            count++;
            currentAgent.simulationState.singleQueryProperties[currentAction][this] = count;
        }
        return count >= this.GetAmount();
    }
}