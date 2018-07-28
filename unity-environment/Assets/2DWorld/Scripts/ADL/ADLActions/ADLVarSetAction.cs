using UnityEngine;

public class ADLVarSetAction : ADLAction{

	public ADLVarSetAction(string name) : base(name)
	{
	}
	
	private string GetPropertyName(){
		return this.GetStringParameter(0);
	}

	private object GetValue(){
		return this.parameters[1].ProcessRPN();
	}

	protected override void Perform(ADLAgent agent){
		switch(this.GetPropertyName())
		{
			case "x":
				agent.transform.localPosition = new Vector3((float) this.GetValue(), agent.transform.localPosition.y, agent.transform.localPosition.z);
				break;
			case "y":
				agent.transform.localPosition = new Vector3(agent.transform.localPosition.x, (float) this.GetValue(), agent.transform.localPosition.z);
				break;
			case "width": {
				Vector2 colliderSize = agent.GetComponent<BoxCollider2D>().size;
				float width = (float) this.GetValue();
				agent.transform.localScale = new Vector3(width / colliderSize.x, agent.transform.localScale.y, 1);
				break;
			}
			case "height": {
				Vector2 colliderSize = agent.GetComponent<BoxCollider2D>().size;
				float height = (float) this.GetValue();
				agent.transform.localScale = new Vector3(agent.transform.localScale.x, height / colliderSize.y, 1);
				break;
			}
			case "lifePoint":
				agent.lifePoint = (float)this.GetValue();
				break;
			case "attack":
				agent.attack = (float)this.GetValue(); 
				break;
			case "isAttacker":
				agent.isAttacker = (bool)this.GetValue();
				break;
			case "isDefender":
				agent.isDefender = (bool)this.GetValue();
				break;
			case "isFlippable":
				agent.isFlippable = (bool)this.GetValue();
				break;
			case "isFlipper":
				agent.isFlipper = (bool)this.GetValue();
				break;
			case "isProjectile":
				agent.isProjectile = (bool)this.GetValue();
				break;
			case "group":
				string groupName = this.GetValue().ToString();
				groupName = groupName.Substring(1, groupName.Length - 2); 
				switch(groupName){
					case "Player":
						agent.group = ADLBaseAgent.Group.Player;
						break;
					case "Enemy":
						agent.group = ADLBaseAgent.Group.Enemy;
						break;
					default:
						break;
				}
				break;
			case "horizontalDirection":
				agent.horizonDirection = ((float) this.GetValue()) > 0 ? ADLBaseAgent.Direction.Normal : ADLBaseAgent.Direction.Inverse;
				break;
			case "verticalDirection":
				agent.verticalDirection = ((float) this.GetValue()) > 0 ? ADLBaseAgent.Direction.Normal : ADLBaseAgent.Direction.Inverse;
				break;
			case "spawnDirection":
				string spawnDirection = this.GetValue().ToString();
				spawnDirection = spawnDirection.Substring(1, spawnDirection.Length - 2);
				switch(spawnDirection){
					case "Player":
						break;
					case "Normal":
						break;
					default:
						break;
				}
				break;
			case "isInvulnerable":
				agent.isInvulnerable = (bool)this.GetValue();
				break;
			case "isHittableByProjectile":
				agent.isHittableByProjectile = (bool)this.GetValue();
				break;
			case "isHittableByEnvironment":
				agent.isHittableByEnvironment = (bool)this.GetValue();
				break;
			case "safeEnvironmentList":
				for (int i = 1; i < this.parameters.Count; i++)
				{
					agent.safeEnvironmentList.Add(this.GetStringParameter(i));
				}
				break;
		}
	}
}
