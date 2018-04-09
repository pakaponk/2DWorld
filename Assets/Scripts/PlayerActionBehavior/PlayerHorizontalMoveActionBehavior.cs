using UnityEngine;

public class PlayerHorizontalMoveActionBehavior : PlayerActionBehavior
{
    public PlayerHorizontalMoveActionBehavior(PlayerController player, string key) : base(player, key) {

    }

    //For Kinematic RigidBody
    public override void actionPerform() {
        float inputValue;
        if (!GameInformation.instance.isControlledByPlayer) {
            inputValue = GameInformation.instance.GetCurrentPlayerInput().horizontalAxis;
        } else {
            inputValue = Input.GetAxis(key);
        }

        if (inputValue > 0.01f) {
            player.horizonDirection = ADLBaseAgent.Direction.Normal;
            player.velocity.x = inputValue * player.maxHorizontalVelocity;
        } else if (inputValue < -0.01f) {
            player.horizonDirection = ADLBaseAgent.Direction.Inverse;
            player.velocity.x = inputValue * player.maxHorizontalVelocity;
        } else {
            player.velocity.x = 0;
        }
    }

    // For Dynamic RigidBody
    // public override void actionPerform()
    // {
    //     Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();
    //     Animator animator = player.GetComponent<Animator>();

    //     if (Input.GetAxis(key) > 0.01f) {
    //         player.horizonDirection = (int) ADLBaseAgent.direction.Normal;
    //         rb2d.velocity = new Vector2(player.maxVelocity * player.horizonDirection, rb2d.velocity.y);
    //     } else if (Input.GetAxis(key) < -0.01f) {
    //         player.horizonDirection = (int) ADLBaseAgent.direction.Inverse;
    //         rb2d.velocity = new Vector2(player.maxVelocity * player.horizonDirection, rb2d.velocity.y);
    //     } else {
    //         rb2d.velocity = new Vector2(0, rb2d.velocity.y);
    //     }

    //     animator.SetFloat("velocityX", Mathf.Abs(Input.GetAxis(key)));
    // }
}