using System.Collections;
using System.Collections.Generic;

public class ADLScript{
	public string agentName;

	public List<ADLState> states;
	public List<ADLScript> subAgentScripts;

	public ADLScript(string name){
		this.InitWithName(name);
	}

	private void InitWithName(string name){
		this.agentName = name;
		this.states = new List<ADLState>();
		this.subAgentScripts = new List<ADLScript>();
	}

	public ADLState FindState(string name){
		return this.states.Find(state => state.name.Equals(name));
	}
}

public interface SpannableAction{
	bool IsEnd();
}

public class ADLCondition{

	public Queue logicalExpressionTokens;
	public int start;
	public int end;

	public ADLCondition(int start){
		this.start = start;
	}

	public bool EvaluateLogicalExpression(){
		return true;
	}
}
