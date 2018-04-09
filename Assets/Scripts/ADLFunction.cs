using System.Collections.Generic;

public abstract class ADLFunction{
	public string name;
	public List<ADLParameter> parameters;

	public ADLFunction(string name){
		this.name = name;
		this.parameters = new List<ADLParameter>();
	}

	public abstract object PerformFunction();

	protected float GetFloatParameter(int parameterIndex){
		return (float) this.parameters[parameterIndex].ProcessRPN();
	}

	protected int GetIntParameter(int parameterIndex){
		return System.Convert.ToInt32(this.parameters[parameterIndex].ProcessRPN());
	}

	protected bool GetBoolParameter(int parameterIndex){
		return (bool) this.parameters[parameterIndex].ProcessRPN();
	}

	protected string GetStringParameter(int parameterIndex){
		string result = (string) this.parameters[parameterIndex].ProcessRPN();
		return result.Substring(1, result.Length - 2);
	}
}