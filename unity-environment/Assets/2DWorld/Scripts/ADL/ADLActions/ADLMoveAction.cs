using UnityEngine;

public class ADLMoveAction : ADLAction, SpannableAction {

	public ADLMoveAction(string name) : base(name){
	
	}

	protected override void Perform(ADLAgent agent){
		agent.velocity = new Vector2(this.GetXVelocity(agent),this.GetYVelocity(agent));
	}

 	bool SpannableAction.IsEnd(){
		return this.GetIsMoveEnd();
	}

	private float GetXVelocity(ADLBaseAgent agent){
		return (int) agent.horizonDirection * this.GetFloatParameter(0);
	}

	private float GetYVelocity(ADLBaseAgent agent)
	{
		return (int) agent.verticalDirection * this.GetFloatParameter(1);
	}

	private bool GetIsMoveEnd(){
		return this.GetBoolParameter(2);
	}
}
