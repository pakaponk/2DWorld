using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class PlayerAgent : Agent {

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
		// Vector2 relaivePosition = Enemy.transform.position - this.player.transform.position;

		// //Relative Position to Enemy
		// AddVectorObs(relaivePosition.x / 8.46f);
		// AddVectorObs(relaivePosition.y / 4.14f);

		// //Player Velocity
		// AddVectorObs(this.player.velocity.x / 5f);
		// AddVectorObs(this.player.velocity.y / 12f);

		//Player & Enemy Life
		AddVectorObs(this.Player.lifePoint / this.playerMaxLifePoint);
		AddVectorObs(this.Enemy.lifePoint / this.enemyMaxLifePoint);
	}

	public override void AgentAction(float[] vectorAction, string textAction) {

		// Discrete Actions
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
		
		// Continuous Actions
		/* float axis =  Mathf.Clamp(vectorAction[0], -1, 1);
		bool isJumpPressed = Mathf.Clamp(vectorAction[1], -1, 1) > 0;
		bool isShootPressed = Mathf.Clamp(vectorAction[2], -1, 1) > 0; */

		//PlayerAction.ActionName actionName = (PlayerAction.ActionName) Enum.ToObject(typeof(PlayerAction.ActionName), selectedAction);
		PlayerInput input = new PlayerInput(this.Player, axis, !prevShootValue && isShootPressed, !prevJumpValue && isJumpPressed, prevJumpValue && !isJumpPressed);

		this.horizontalAction.Perform(input);
		this.jumpAction.Perform(input);
		this.shootAction.Perform(input);

		this.prevJumpValue = isJumpPressed;
		this.prevShootValue = isShootPressed;

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
		this.Player.transform.position = new Vector3(-6.42f, 0, 0);
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
}
