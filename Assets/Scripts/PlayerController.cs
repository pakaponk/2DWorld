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
	private PlayerActionBehavior horizontalBehavior;
	private PlayerActionBehavior leftBehavior;
	private PlayerActionBehavior rightBehavior;
	private PlayerActionBehavior button1Behavior;
	private PlayerActionBehavior button2Behavior;
	private PlayerActionBehavior button3Behavior;
	private PlayerActionBehavior button4Behavior;
	#endregion 

	#region Action SelectedIndexs Properties
	[HideInInspector]
	public int rightActionSelectedIndex;
	[HideInInspector]
	public int leftActionSelectedIndex;
	
	public int button1ActionSelectedIndex = 2;
	
	public int button2ActionSelectedIndex = 3;
	[HideInInspector]
	public int button3ActionSelectedIndex;
	[HideInInspector]
	public int button4ActionSelectedIndex;
	#endregion

	#region Player ShootProjectile Action Behavior - Necessary Properties
	public int bulletLimit = 3;
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
		horizontalBehavior = new PlayerHorizontalMoveActionBehavior(this, "Horizontal");
		button1Behavior = InitializePlayerActionBehavior(button1ActionSelectedIndex,"Jump");
		button2Behavior = InitializePlayerActionBehavior(button2ActionSelectedIndex,"Fire1");
	}
	
	void Update() {
		if (shootingAnimationRemaningTime > 0) 
			shootingAnimationRemaningTime -= Time.deltaTime;

		/*
		 * Movement Control
		 */
		horizontalBehavior.actionPerform();
		/*
		 * Action Control (A B L1 R1)
		 */ 
		button1Behavior.actionPerform();
		button2Behavior.actionPerform();

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

		if (!GameInformation.instance.isControlledByPlayer) {
			GameInformation.PlayerInput input = GameInformation.instance.GetCurrentPlayerInput();
			Debug.Log($"{this.rb2d.position.x} {this.rb2d.position.y} {input.x} {input.y}");
			GameInformation.instance.RemoveCurrentPlayerInput();
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		
		if (!this.IsAlive()) {
			GameInformation.instance.End();
		}
	}

	private PlayerActionBehavior InitializePlayerActionBehavior(int actionIndex,string key){
		switch(actionIndex)
		{
			case(0) :
				return new PlayerMoveLeftActionBehavior(this,key);
			case(1)	:
				return new PlayerMoveRightActionBehavior(this,key);
			case(2) :
				return new PlayerJumpBehavior(this,key);
			case(3) :
				return new PlayerShootProjectileActionBehavior(this,key);
			default :
				return new PlayerMoveLeftActionBehavior(this,key);
		}
	}

	/*
	 * Can be removed all references to this method is MoveLeft and MoveRight which is deprecated
	 */
	public Rigidbody2D getRigidBody2D(){
		return this.rb2d;
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
}

