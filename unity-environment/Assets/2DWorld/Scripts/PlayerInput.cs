using System;
using UnityEngine;

[Serializable]
public class PlayerInput{
    public float horizontalAxis;
    public bool isFireButtonDown;
    public bool isJumpButtonDown;
    public bool isJumpButtonUp;

    public PlayerInput(ADLBaseAgent player, float horizontalAxis, bool isFireButtonDown, bool isJumpButtonDown, bool isJumpButtonUp) {
        this.horizontalAxis = horizontalAxis;
        this.isFireButtonDown = isFireButtonDown;
        this.isJumpButtonDown = isJumpButtonDown;
        this.isJumpButtonUp = isJumpButtonUp;
    }
}