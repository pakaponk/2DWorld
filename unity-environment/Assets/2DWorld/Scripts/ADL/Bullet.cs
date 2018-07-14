using UnityEngine;

public class Bullet : ADLBaseAgent{

	public static int limit = 3;
	public static int total = 0;

	private PlayerController player;

	/// <summary>
	///	The owner of this bullet
	/// </summary>
	public PlayerController Player { get; set; }

	public float horizontalMoveVelocity = 8f;

	// Use this for initialization
	public new void Start () {
		base.Start();

		this.gravityModifier = 0f;
		//this.rb2d.velocity = new Vector2(horizontalMoveVelocity * this.horizonDirection, 0);
	}

	void Update() {
		velocity.x = horizontalMoveVelocity * (int) this.horizonDirection;
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		this.Player.DecreaseTotalBullet();
	}
}
