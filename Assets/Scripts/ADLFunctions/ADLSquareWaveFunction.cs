using System;

public class ADLSquareWaveFunction: ADLFunction{

    public ADLSquareWaveFunction(string name) : base(name)
	{
		
	}

    private float getAmplitude(){
        return this.GetFloatParameter(0);
    }

	private float getFrequency(){
		return this.GetFloatParameter(1);
	}

    public override object PerformFunction(){
		float timePassed = ADLAgent.currentUpdatingAgent.simulationState.elapsedTimes[ADLAction.performingAction];
		return this.getAmplitude() * (float) Math.Pow(-1, (Math.Floor(timePassed / this.getFrequency()) % 2));
	}
}