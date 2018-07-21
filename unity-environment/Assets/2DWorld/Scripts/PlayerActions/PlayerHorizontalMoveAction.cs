using UnityEngine;

public class PlayerHorizontalMoveAction : PlayerAction
{
    public PlayerHorizontalMoveAction(PlayerController player, string key) : base(player, key) {

    }

    //For Kinematic RigidBody
    public override void Perform() {
        float inputValue;
        if (!GameRecorder.instance.isControlledByPlayer) {
            inputValue = GameRecorder.instance.GetCurrentPlayerInput().horizontalAxis;
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

    public override void Perform(PlayerInput input) {
        float axis = input.horizontalAxis;

        if (axis > 0.01f) {
            player.horizonDirection = ADLBaseAgent.Direction.Normal;
            player.velocity.x = axis * player.maxHorizontalVelocity;
        } else if (axis < -0.01f) {
            player.horizonDirection = ADLBaseAgent.Direction.Inverse;
            player.velocity.x = axis * player.maxHorizontalVelocity;
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