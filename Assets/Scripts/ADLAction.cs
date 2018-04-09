using System.Collections.Generic;

public abstract class ADLAction{
	public string name;
	public List<ADLParameter> parameters;

	public static ADLAction performingAction;

	public ADLAction(string name){
		this.name = name;
		this.parameters = new List<ADLParameter>();
	}

	public void PerformAction(ADLAgent agent) {
		ADLAction.performingAction = this;
		this.Perform(agent);
	}

	protected abstract void Perform(ADLAgent agent);

	protected float GetFloatParameter(int parameterIndex){
		return (float) this.parameters[parameterIndex].ProcessRPN();
	}

	protected int GetIntParameter(int parameterIndex){
		return System.Convert.ToInt32(this.parameters[parameterIndex].ProcessRPN());
	}

	protected bool GetBoolParameter(int parameterIndex){
		object val = this.parameters[parameterIndex].ProcessRPN();
		return (bool) val;
	}

	protected string GetStringParameter(int parameterIndex){
		string result = (string) this.parameters[parameterIndex].ProcessRPN();
		return result.Substring(1, result.Length - 2);
	}
}