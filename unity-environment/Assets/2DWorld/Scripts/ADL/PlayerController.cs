using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerController : ADLBaseAgent {
	public Text agentLifePointText;

	public float maxHorizontalVelocity = 5f;
	public float initialJumpVelocity = 6f;
	
	public bool isShootable = true;

	public GameObject bullet;

	#region Player Action Behaviors Properties
	private PlayerAction horizontalAction;
	private PlayerAction leftButtonAction;
	private PlayerAction rightButtonAction;
	private PlayerAction button1Action;
	private PlayerAction button2Action;
	private PlayerAction button3Action;
	private PlayerAction button4Action;
	#endregion 

	#region Action name for each button
	[HideInInspector]
	public PlayerAction.ActionName rightActionName;
	[HideInInspector]
	public PlayerAction.ActionName leftActionName;
	
	public PlayerAction.ActionName button1ActionName;
	
	public PlayerAction.ActionName button2ActionName;
	[HideInInspector]
	public int button3ActionSelectedIndex;
	[HideInInspector]
	public int button4ActionSelectedIndex;
	#endregion

	#region Player ShootProjectile Action Behavior - Necessary Properties
	public int bulletLimit = 3;
	public int totalBullet = 0;
	public float shootFrequency = 0.04f;
	public float shootingAnimationRemaningTime = 0.0f;
	public float shootingAnimationTimePeriod = 0.5f;
	#endregion

	private const float MIN_GROUND_NORMAL_Y = 0.65f;

	public new void Awake(){
		base.Awake();

		this.agentName = "Player";
	}

	// Use this for initialization
	public new void Start () {
		base.Start();

		// Camera myCamera = FindObjectOfType<Camera> ();
		// myCamera.orthographicSize = (1080 * 0.01f) / 2.0f;
		if (this.agentLifePointText != null) {
			this.agentLifePointText.text = "LP: " + this.lifePoint;
		}

		//Initialize ActionBehavior
		horizontalAction = new PlayerHorizontalMoveAction(this, "Horizontal");
		button1Action = PlayerAction.CreatePlayerAction(button1ActionName, this, "Jump");
		button2Action = PlayerAction.CreatePlayerAction(button2ActionName, this, "Fire1");
	}
	
	void Update() {
		if (shootingAnimationRemaningTime > 0) 
			shootingAnimationRemaningTime -= Time.deltaTime;

		/*
		 * Movement Control
		 */
		//horizontalAction.Perform();
		/*
		 * Action Control (A B L1 R1)
		 */ 
		//button1Action.Perform();
		//button2Action.Perform();

		UpdateAnimation();

		if (GameInformation.instance != null && !GameInformation.instance.isControlledByPlayer) {
			PlayerInput input = GameInformation.instance.GetCurrentPlayerInput();
			GameInformation.instance.RemoveCurrentPlayerInput();
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		
		if (!this.IsAlive()) {
			GameInformation.instance?.End();
		}
	}

	/// <summary>
	/// This method is deprecated. All references to this method is MoveLeft and MoveRight which is deprecated as well.
	/// </summary>
	public Rigidbody2D getRigidBody2D(){
		return this.rb2d;
	}

	private void UpdateAnimation() {
		GetComponent<SpriteRenderer>().flipX = this.horizonDirection < 0 ? true : false;
		
		Animator animator = GetComponent<Animator>();
		animator.SetFloat("velocityX", Math.Abs(velocity.x) / maxHorizontalVelocity);

		if (shootingAnimationRemaningTime < 0.01f) {
			animator.SetBool("isShooting", false);
		} else {
			animator.SetBool("isShooting", true);
		}

		if (velocity.y > 0.01f || velocity.y < -0.01f) {
			animator.SetBool("isFloating", true);
		} else {
			animator.SetBool("isFloating", false);
		}
	}

	public override bool DecreaseLifePoint(ADLBaseAgent agent){
		bool isLifePointDecreased = base.DecreaseLifePoint(agent);

		if (isLifePointDecreased)
		{
			if (this.agentLifePointText != null) {
				if (this.IsAlive())
					this.agentLifePointText.text = "LP: " + this.lifePoint;
				else
					this.agentLifePointText.text = "LP: " + 0;
			}
			StartCoroutine(Fade());
		}

		return isLifePointDecreased;

	}

	private IEnumerator Fade(){
		this.SetLayer((int) Layer.Invulnerable);
		this.isInvulnerable = true;
		for (int i = 0;i < 10;i++)
		{
			this.ToggleSpriteColor();
			yield return new WaitForSeconds(0.25f);
		}
		this.SetLayer((int) Layer.Player);
		this.isInvulnerable = false;
	}

	private void ToggleSpriteColor(){
		SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
		if (spriteRenderer.color.Equals(Color.white))
		{
			spriteRenderer.color = Color.red;
		}
		else
		{
			spriteRenderer.color = Color.white;
		}
	}

	public void IncreaseTotalBullet() {
		this.totalBullet += 1;
	}

	public void DecreaseTotalBullet() {
		if (this.totalBullet > 0) 
			this.totalBullet -= 1;

		if (this.totalBullet < 0)
			this.totalBullet = 0;
	}

	public bool IsRunOutOfBullet() {
		return this.totalBullet < this.bulletLimit;
	}
}

