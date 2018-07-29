using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BossFightAcademy : Academy {
    public Vector3[] playerInitialPositions;

    private System.Random random;

    void Start() {
        random = new System.Random();
    }

    public Vector3 RandomInitialPosition() {
        int size = playerInitialPositions.Length;
        return playerInitialPositions[random.Next(size)];
    }
}
