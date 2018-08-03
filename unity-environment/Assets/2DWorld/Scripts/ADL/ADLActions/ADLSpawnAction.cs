using System;
using UnityEngine;

public class ADLSpawnAction : ADLAction{
    public ADLSpawnAction(string name): base(name){

    }

    private string getAgentName(){
        return this.GetStringParameter(0);
    }

    private float getX(){
        return this.GetFloatParameter(1);
    }

    private float getY(){
        return this.GetFloatParameter(2);
    }

    private float getWidth(){
        return this.GetFloatParameter(3);
    }

    private float getHeight(){
        return this.GetFloatParameter(4);
    }

    private string getSpawnDirection() {
        return this.GetStringParameter(5);
    }

    protected override void Perform(ADLAgent agent){
        //Find SubAgentScript by name
        string spawnAgentName = this.getAgentName();
        string spawnDirection = this.getSpawnDirection();
        
        ADLScript subAgentScript = agent.agentScript.subAgentScripts.Find(
            script => script.agentName.Equals(spawnAgentName));

        //Instantiate new Agent Object and Assign SubAgentScript to it
        GameObject projectile = GameObject.Instantiate(agent.agentPrefab) as GameObject;
        SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;

        ADLAgent subAgent = projectile.GetComponent<ADLAgent>();
        subAgent.isInitStateExecuted = false;
        subAgent.agentScript = subAgentScript;
        subAgent.agentScript.subAgentScripts = agent.agentScript.subAgentScripts;

        Vector2 colliderSize = projectile.GetComponent<BoxCollider2D>().size;
        projectile.transform.localPosition = agent.transform.localPosition;
        
        if (spawnDirection.Equals("TowardPlayer")) {
            try {
                ADLBaseAgent player = ADLBaseAgent.FindAgent("Player", agent.transform.parent);
                if (player.transform.localPosition.x > agent.transform.localPosition.x) {
                    SetProjectilePositionAndDirection(subAgent, ADLBaseAgent.Direction.Normal);
                } else {
                    SetProjectilePositionAndDirection(subAgent, ADLBaseAgent.Direction.Inverse);
                }
            } catch (Exception e) when (e is NullReferenceException || e is MissingReferenceException) {
                Debug.LogError("Player Not Found: " + e.Message);
                SetProjectilePositionAndDirection(subAgent, agent.horizonDirection);
            }
        } else {
            SetProjectilePositionAndDirection(subAgent, agent.horizonDirection);
        }
        projectile.transform.localScale = new Vector3(this.getWidth() / colliderSize.x, this.getHeight() / colliderSize.y);

        subAgent.Start();
    }

    private void SetProjectilePositionAndDirection(ADLAgent projectile, ADLBaseAgent.Direction direction) {
        if (direction.Equals(ADLBaseAgent.Direction.Normal)) {
            projectile.transform.localPosition += new Vector3(this.getX(), this.getY(), 0);
            projectile.horizonDirection = ADLBaseAgent.Direction.Normal;
        } else {
            projectile.transform.localPosition += new Vector3(-this.getX(), this.getY(), 0);
            projectile.horizonDirection = ADLBaseAgent.Direction.Inverse;
        }
    }
}