public class ADLWaitAction : ADLAction, SpannableAction{
    public ADLWaitAction(string name): base(name){

    }

    private int getMilliseconds(){
        return GetIntParameter(0);
    }

    protected override void Perform(ADLAgent agent){
        //Do Nothing
    }

    bool SpannableAction.IsEnd(){
        return ADLAgent.currentUpdatingAgent.simulationState.elapsedTimes[this] * 1000 >= this.getMilliseconds();
	}
}