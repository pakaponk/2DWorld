

public abstract class PlayerAction
{
	public enum ActionName: int { MoveLeft = 0, MoveRight = 1, HorizontalMove = 2, Jump = 3, ShootProjectile = 4 }

	protected PlayerController player;
	protected string key;

	protected PlayerAction(PlayerController player, string key) {
		this.player = player;
		this.key = key;
	}

	public abstract void Perform();

	public virtual void Perform(PlayerInput input) {
		
	}

	public static PlayerAction CreatePlayerAction(ActionName name, PlayerController player, string key) {
		switch (name) {
			case ActionName.HorizontalMove:
				return new PlayerHorizontalMoveAction(player, key);
			case ActionName.Jump:
				return new PlayerJumpAction(player, key);
			case ActionName.ShootProjectile:
				return new PlayerShootProjectileAction(player, key);
			default:
				return null;
		}
	}
}




