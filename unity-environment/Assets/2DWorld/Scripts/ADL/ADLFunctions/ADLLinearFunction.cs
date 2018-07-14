
public class ADLLinearFunction : ADLFunction{

	public ADLLinearFunction(string name) : base(name) {
	}

	private float getSlope(){
		return this.GetFloatParameter(0);
	}

	private float getConstant(){
		return this.GetFloatParameter(1);
	}

	public override object PerformFunction(){
		float timePassed = ADLAgent.currentUpdatingAgent.simulationState.elapsedTimes[ADLAction.performingAction];
		return (this.getSlope() * timePassed) + this.getConstant();
	}
}
