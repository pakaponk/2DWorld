using UnityEngine;
using System.Collections.Generic;

public abstract class ADLBaseAgent : PhysicsObject{
    public static readonly Dictionary<Transform, List<ADLBaseAgent>> agents = new Dictionary<Transform, List<ADLBaseAgent>>();
    public static ADLBaseAgent FindAgent(string name, Transform parent){
		if (name.Equals("Self"))
			return ADLAgent.currentUpdatingAgent;
		else
			return agents[parent].Find(agent => agent.agentName.Equals(name));
	}

	public string agentName; 
	public float lifePoint;
	public float attack;
	public bool isAttacker;
	public bool isDefender;
	public bool isFlippable;
	public bool isFlipper;
	public bool isProjectile;
	public bool isInvulnerable;
	public bool isHittableByProjectile;
	public bool isHittableByEnvironment;
	public Group group; // Is it on player side or enemy side
	public Direction horizonDirection = Direction.Normal;
	public Direction verticalDirection = Direction.Normal;
	public enum Direction: int { Normal=1, Inverse=-1};
	public enum Group: int { Player=0, Enemy=1, Environment=2 };
	protected enum Layer: int { Player=8, Enemy=9, PlayerProjectile=10, EnemyProjectile=11, NeutralProjectile=12, Environment=13, Invulnerable=14, EnemyAndNonEnvCollidableProjectile=15}
	public List<ADLBaseAgent> collisionList = new List<ADLBaseAgent>();
	public HashSet<string> safeEnvironmentList = new HashSet<string>(); 

	public Vector2 prevPosition;
	public Vector2 prevVelocity;

    public void Awake(){
		List<ADLBaseAgent> list;
		if (agents.ContainsKey(this.transform.parent)) {
			list = ADLBaseAgent.agents[this.transform.parent];
		}
		else {
			list = new List<ADLBaseAgent>();
			ADLBaseAgent.agents.Add(this.transform.parent, list);
		}
		list.Add(this);
	}

	protected new void Start(){
		this.InitializeLayer();
		base.Start();
	}

	protected new void FixedUpdate() {
		this.prevPosition = this.rb2d.position;
		this.prevVelocity = this.velocity;
		
		base.FixedUpdate();
		
		if (!(this is ADLAgent)) {
			this.collisionList.Clear();
		}
	}

	protected virtual void OnDestroy(){
		ADLBaseAgent.agents[this.transform.parent].Remove(this);
	}

	public bool Attack(ADLBaseAgent agent){
		if (agent.IsEnvironment()){
			return false;
		}

		if (this.isProjectile)
		{
			if (this.group != agent.group)
			{
				return agent.DecreaseLifePoint(this);
			}
			return false;
		}
		else
		{
			return agent.DecreaseLifePoint(this);
		}
	}

	public void Flip(ADLBaseAgent agent){
		if (this.isFlipper && agent.isFlippable)
		{
			if (agent.prevVelocity.x > 0.01 || agent.prevVelocity.x < -0.01) {
				Debug.Log("Flip X");
				agent.horizonDirection = GetOppositeDirection(agent.horizonDirection);
			}
			if (agent.prevVelocity.y > 0.01 || agent.prevVelocity.y < -0.01) {
				Debug.Log("Flip Y");
				agent.verticalDirection = GetOppositeDirection(agent.verticalDirection);
			}
		}
	}

	public static Direction GetOppositeDirection(Direction direction) {
		if (direction.Equals(Direction.Normal)) {
			return Direction.Inverse;
		}
		else {
			return Direction.Normal;
		}
	}

	public virtual bool DecreaseLifePoint(ADLBaseAgent agent)
	{
		if (!this.isInvulnerable && (agent.isAttacker && this.isDefender))
		{
			this.lifePoint -= agent.attack;
			return agent.attack > 0;
		}
		return false;
	}

	public bool IsAlive(){
		return this.lifePoint > 0;
	}

	public bool IsEnvironment(){
		return this.gameObject.layer == (int) Layer.Environment;
	}

	void OnCollisionEnter2D(Collision2D coll){
		//Debug.Log("Collision between " + this.gameObject.name + " " + coll.gameObject.name);

		ADLBaseAgent agent = coll.gameObject.GetComponent<ADLBaseAgent>();
		this.collisionList.Add(agent);
		
		this.Flip(agent);

		//Attack Event
		if (this.Attack(agent)){
			if (!agent.IsAlive()) {
				if (agent is PlayerController && agent.gameObject.GetComponent<RockmanVisualAgent>() != null) {
					// Do Nothing
				} else {
					Destroy(agent.gameObject);
				}
			}
		}

		if (this.isProjectile && agent.IsEnvironment())
		{
			if (!this.isFlippable || !agent.isFlipper)
			{
				if (this.isHittableByEnvironment && !this.safeEnvironmentList.Contains(agent.agentName)) {
					Destroy(this.gameObject);
				}
			}
		}
	}

	private void InitializeLayer() {
		if (this.isProjectile)
		{
			if (this.isHittableByProjectile)
			{
				this.gameObject.layer = (int) Layer.NeutralProjectile;
			}
			else
			{
				if (this.group == (int) Group.Player)
				{
					this.gameObject.layer = (int) Layer.PlayerProjectile;
				}
				else
				{
					this.gameObject.layer = (int) Layer.EnemyProjectile;
					if (!this.isHittableByEnvironment) {
						this.gameObject.layer = (int) Layer.EnemyAndNonEnvCollidableProjectile;
					}
				}
			}
		}
		else
		{
			if (this.group == (int) ADLBaseAgent.Group.Player)
			{
				this.gameObject.layer = (int) Layer.Player;
			}
			else
			{
				this.gameObject.layer = (int) Layer.Enemy;
			}
		}
	}
}
