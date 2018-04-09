using UnityEngine;

public class ADLMoveTowardAction : ADLAction, SpannableAction {

	public ADLMoveTowardAction(string name) : base(name){
	
	}

	protected override void Perform(ADLAgent agent){
        Vector2 directionVector;
        if (ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[this].ContainsKey(this)) {
            directionVector = (Vector2) ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[this][this];
        } else {
            Vector2 position = agent.GetComponent<Rigidbody2D>().position;
            Vector2 target = new Vector2(this.GetX(), this.GetY());
            directionVector = (target - position).normalized;
            ADLAgent.currentUpdatingAgent.simulationState.singleQueryProperties[this].Add(this, directionVector);
        }

        agent.velocity = directionVector * this.GetVelocity();
	}

 	bool SpannableAction.IsEnd(){
		return this.GetIsMoveEnd();
	}

	private float GetX() {
		return this.GetFloatParameter(0);
	}

	private float GetY() {
		return this.GetFloatParameter(1);
	}

    private float GetVelocity() {
        return this.GetFloatParameter(2);
    }

	private bool GetIsMoveEnd() {
		return this.GetBoolParameter(3);
	}
}
