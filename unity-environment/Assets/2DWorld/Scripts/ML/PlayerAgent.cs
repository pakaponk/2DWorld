using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class PlayerAgent : Agent {

	protected struct AgentInput {
		public float axis;
		public bool isJumpPressed;
		public bool isShootPressed;

		public AgentInput(float axis, bool isJumpPressed, bool isShootPressed) {
			this.axis = axis;
			this.isJumpPressed = isJumpPressed;
			this.isShootPressed = isShootPressed;
		}
	}

	private const float X_RANGE = 8.42f;
	private const float Y_RANGE = 4.14f;
	private const float MAX_X_DISTANCE = X_RANGE * 2;
	private const float MAX_Y_DISTANCE = Y_RANGE * 2;

	private PlayerController Player;
    private RayPerception rayPerception;

	public ADLAgent Enemy;
	public GameObject EnemyPrefab;
	public GameObject PlayerPrefab;

	private float playerMaxLifePoint;
	private float enemyMaxLifePoint;

	private PlayerAction horizontalAction;
	private PlayerAction jumpAction;
	private PlayerAction shootAction;

	private bool prevJumpValue;
	private bool prevShootValue;
	private float prevPlayerLifePoint;
	private float prevEnemyLifePoint;
	
	// Use this for initialization
	void Start () {
		this.Player = this.GetComponent<PlayerController>();
        this.rayPerception = this.GetComponent<RayPerception>();
        this.playerMaxLifePoint = this.Player.lifePoint;
		this.enemyMaxLifePoint = this.Enemy.lifePoint;

		this.horizontalAction = PlayerAction.CreatePlayerAction(PlayerAction.ActionName.HorizontalMove, this.Player, "Horizontal");
		this.jumpAction = PlayerAction.CreatePlayerAction(PlayerAction.ActionName.Jump, this.Player, "Jump");
		this.shootAction = PlayerAction.CreatePlayerAction(PlayerAction.ActionName.ShootProjectile, this.Player, "Fire1");

		this.prevJumpValue = false;
		this.prevShootValue = false;
		this.prevPlayerLifePoint = this.Player.lifePoint;
		this.prevEnemyLifePoint = this.Enemy.lifePoint;
	}

	public override void CollectObservations() {
		Vector2 playerPosition = this.Player.transform.localPosition;
		Vector2 enemyPosition = this.Player.transform.localPosition;
		Vector2 relativeDistance = enemyPosition - playerPosition;

		// Player Position
		AddVectorObs(playerPosition.x / X_RANGE);
		AddVectorObs(playerPosition.y / Y_RANGE);
		// Enemy Position
		AddVectorObs(enemyPosition.x / X_RANGE);
		AddVectorObs(enemyPosition.y / Y_RANGE);

		// Relative Distance to Enemy
		AddVectorObs(Math.Abs(relativeDistance.x) / MAX_X_DISTANCE);
		AddVectorObs(Math.Abs(relativeDistance.y) / MAX_Y_DISTANCE);

		// Player Velocity
		AddVectorObs(this.Player.velocity.x / 5f);
		AddVectorObs(this.Player.velocity.y / 12f);
		// Enemy Velocity
		AddVectorObs(this.Enemy.velocity.x / 20f);
		AddVectorObs(this.Enemy.velocity.y / 20f);	

		// Player & Enemy Life
		AddVectorObs(this.Player.lifePoint / this.playerMaxLifePoint);
		AddVectorObs(this.Enemy.lifePoint / this.enemyMaxLifePoint);
	}

	public override void AgentAction(float[] vectorAction, string textAction) {
		AgentInput agentInput = this.ConvertDiscreteVectorActionToAgentInput(vectorAction);

		PlayerInput playerInput = new PlayerInput(
			this.Player, 
			agentInput.axis, 
			!prevShootValue && agentInput.isShootPressed, 
			!prevJumpValue && agentInput.isJumpPressed, 
			prevJumpValue && !agentInput.isJumpPressed
		);
		
		this.horizontalAction.Perform(playerInput);
		this.jumpAction.Perform(playerInput);
		this.shootAction.Perform(playerInput);

		this.prevJumpValue = agentInput.isJumpPressed;
		this.prevShootValue = agentInput.isShootPressed;

		// Reward when the environment end
		if (Enemy.lifePoint <= 0) {
			AddReward(1f);
			//Debug.Log("Trigger Enemy Reset: " + Enemy.lifePoint);
			Done();
		}
		else if (Player.lifePoint <= 0) {
			AddReward(-1f);
			Done();
		}
		
		// Reward for each step
		AddReward(-0.0001f);

		// Reward when got damaged
		if (this.prevPlayerLifePoint > this.Player.lifePoint) {
			AddReward(-((this.prevPlayerLifePoint - this.Player.lifePoint) / this.playerMaxLifePoint));
		}

		// Reward when able to damage the enemy
		if (this.prevEnemyLifePoint > this.Enemy.lifePoint) {
			AddReward(((this.prevEnemyLifePoint - this.Enemy.lifePoint) / this.enemyMaxLifePoint) * 0.5f);
		}

		this.prevPlayerLifePoint = this.Player.lifePoint;
		this.prevEnemyLifePoint = this.Enemy.lifePoint;
	}

	void Update() {
		RequestDecision();
	}

	public override void AgentReset() {
		if (Enemy.lifePoint <= 0) {
			ResetEnemy(false);
			ResetPlayer();
		}

		else if (Player.lifePoint <= 0) {
			ResetPlayer();
			ResetEnemy(true);
		}

		else {
			ResetPlayer();
			ResetEnemy(true);
		}

		Bullet[] bullets = this.transform.parent.gameObject.GetComponentsInChildren<Bullet>();
		foreach (Bullet bullet in bullets)
		{
			Destroy(bullet.gameObject);
		}
	}

	private void ResetPlayer() {
		this.Player.transform.localPosition = FindObjectOfType<BossFightAcademy>().RandomInitialPosition();
		this.Player.lifePoint = this.playerMaxLifePoint;
		this.Player.agentLifePointText.text = "LP: " + this.Player.lifePoint;
		this.Player.isGrounded = false;
		this.Player.velocity = new Vector2(0, 0);
		this.Player.horizonDirection = ADLBaseAgent.Direction.Normal;
		this.Player.verticalDirection = ADLBaseAgent.Direction.Normal;

		this.prevJumpValue = false;
		this.prevShootValue = false;
		this.prevPlayerLifePoint = this.Player.lifePoint;
	}

	private void ResetEnemy(bool isEnemyAlive) {
		if (isEnemyAlive) {
			this.Enemy.isInitStateExecuted = false;
			this.Enemy.Start();
		} else {
			Text enemyLifePointText = this.Enemy.agentLifePointText;
			GameObject enemy = Instantiate(EnemyPrefab, this.transform.parent);
			this.Enemy = enemy.GetComponent<ADLAgent>();
			this.Enemy.agentLifePointText = enemyLifePointText;
		}
	}

	private AgentInput ConvertDiscreteVectorActionToAgentInput(float[] vectorAction) {
		float axis;
		bool isJumpPressed;
		bool isShootPressed;
		
		int action = Mathf.FloorToInt(vectorAction[0]);
		
		switch (action) {
			case 1: //000
				axis = -1f;
				isJumpPressed = false;
				isShootPressed = false;
				break;
			case 2: //001
				axis = -1f;
				isJumpPressed = false;
				isShootPressed = true;
				break;
			case 3: //010
				axis = -1f;
				isJumpPressed = true;
				isShootPressed = false;
				break;
			case 4: //011
				axis = -1f;
				isJumpPressed = true;
				isShootPressed = true;
				break;
			case 5: //100
				axis = 0f;
				isJumpPressed = false;
				isShootPressed = false;
				break;
			case 6: //101
				axis = 0f;
				isJumpPressed = false;
				isShootPressed = true;
				break;
			case 7: //110
				axis = 0f;
				isJumpPressed = true;
				isShootPressed = false;
				break;
			case 8: //111
				axis = 0f;
				isJumpPressed = true;
				isShootPressed = true;
				break;
			case 9: //200
				axis = 1f;
				isJumpPressed = false;
				isShootPressed = false;
				break;
			case 10: //201
				axis = 1f;
				isJumpPressed = false;
				isShootPressed = true;
				break;
			case 11: //210
				axis = 1f;
				isJumpPressed = true;
				isShootPressed = false;
				break;
			default: //211
				axis = 1f;
				isJumpPressed = true;
				isShootPressed = true;
				break;
		}

		return new AgentInput(axis, isJumpPressed, isShootPressed);
	}
	private AgentInput ConvertContinuousVectorActionToAgentInput(float[] vectorAction) {
		float axis =  Mathf.Clamp(vectorAction[0], -1, 1);
		bool isJumpPressed = Mathf.Clamp(vectorAction[1], -1, 1) > 0;
		bool isShootPressed = Mathf.Clamp(vectorAction[2], -1, 1) > 0;

		return new AgentInput(axis, isJumpPressed, isShootPressed);
	}
}
