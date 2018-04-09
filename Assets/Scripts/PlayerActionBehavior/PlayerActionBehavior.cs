

public abstract class PlayerActionBehavior
{
	protected PlayerController player;
	protected string key;

	protected PlayerActionBehavior(PlayerController player, string key) {
		this.player = player;
		this.key = key;
	}

	public abstract void actionPerform();
}




