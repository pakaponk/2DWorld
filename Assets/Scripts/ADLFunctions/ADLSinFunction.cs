using System;

public class ADLSinFunction: ADLFunction{
    public ADLSinFunction(string name) : base(name)
	{
	}

    private float getAmplitude(){
        return this.GetFloatParameter(0);
    }

    private float getTangentialSpeed(){
        return this.GetFloatParameter(1);
    }   

    private float getPhase(){
        return this.GetFloatParameter(2);
    }

    public override object PerformFunction(){
        float timePassed = ADLAgent.currentUpdatingAgent.simulationState.elapsedTimes[ADLAction.performingAction];
        return this.getAmplitude() * (float) Math.Sin((this.getTangentialSpeed() * timePassed) + this.getPhase());   
    }
}