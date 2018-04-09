public class Bullet : ADLBaseAgent{

	public static int limit = 3;
	public static int total = 0;

	public float horizontalMoveVelocity = 8f;

	// Use this for initialization
	public new void Start () {
		base.Start();

		this.gravityModifier = 0f;
		//this.rb2d.velocity = new Vector2(horizontalMoveVelocity * this.horizonDirection, 0);

		Bullet.IncreaseTotalBullet();
	}

	void Update() {
		velocity.x = horizontalMoveVelocity * (int) this.horizonDirection;
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		Bullet.DecreaseTotalBullet();
	}

	private static void IncreaseTotalBullet() {
		Bullet.total += 1;
	}

	private static void DecreaseTotalBullet() {
		if (Bullet.total > 0) 
			Bullet.total -= 1;
	}
}
