using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class ADLAgent : ADLBaseAgent {

	public bool isInitStateExecuted = false;
	public TextAsset agentScriptFile;

	public Text agentLifePointText;

	public SimulationState simulationState;

	public ADLScript agentScript;
	public ADLState CurrentState { get; set; }

	public GameObject agentPrefab;

	public Rigidbody2D GetRigidbody2D(){
		return GetComponent<Rigidbody2D>();
	}

	public static ADLAgent currentUpdatingAgent;

	// Use this for initialization
	public new void Start () {
		if (!this.isInitStateExecuted) {
			this.CompileScript();
			
			this.agentName = this.agentScript.agentName;
			this.gameObject.name = this.agentName;

			ADLState initState = this.agentScript.states.Find((state) => state.name.Equals("init"));

			if (initState != null)
			{
				foreach (ADLAction action in initState.seqs[0].actions)
				{
					action.PerformAction(this);
				}

				ADLState firstState = this.agentScript.states.Find((state) => !state.name.Equals("init") && !state.name.Equals("des"));
				if (firstState != null) {
					this.simulationState = new SimulationState(this.agentScript.states[1]);
				} else {
					throw new Exception("Unable to find the first state after init state");
				}
			}

			base.Start();

			if (agentLifePointText != null) {
				agentLifePointText.text = "LP: " + this.lifePoint;
			}

			this.isInitStateExecuted = true;
		}
	}
	
	// Update is called once per frame
	protected new void FixedUpdate () {
		ADLAgent.currentUpdatingAgent = this;

		this.PerformAction();

		base.FixedUpdate();

		this.UpdateSimulationState();

		this.collisionList.Clear();

		if (this.isProjectile && !this.isHittableByEnvironment) {
			if (this.rb2d.position.x < -12 || this.rb2d.position.x > 12 ||
				this.rb2d.position.y < -8 || this.rb2d.position.y > 8) {
				Destroy(this.gameObject);
			}
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		if (this.group.Equals(Group.Enemy) && !this.isProjectile && !this.IsAlive()) {
			GameInformation.instance.End();
		}
	}

	private void PerformAction() {
		foreach (ADLSequence seq in this.simulationState.currentState.seqs)
		{
			ADLAction action = (ADLAction) seq.actions[this.simulationState.sequenceIndexes[seq.name]];
			
			if (this.simulationState.elapsedTimes.ContainsKey(action)) {
				this.simulationState.elapsedTimes[action] += Time.fixedDeltaTime;
			} else {
				this.simulationState.elapsedTimes.Add(action, Time.fixedDeltaTime);
				this.simulationState.singleQueryProperties.Add(action, new Dictionary<object, object>());
			}
			
			if (!(action is ADLToStateAction) && !(action is ADLConditionAction)) {
				action.PerformAction(this);
			}
		}
	}

	private void UpdateSimulationState() {
		foreach (ADLSequence seq in this.simulationState.currentState.seqs)
		{
			int currentSequenceIndex = this.simulationState.sequenceIndexes[seq.name];
			ADLAction action = seq.actions[currentSequenceIndex];
			if (action is SpannableAction) {
				if (((SpannableAction) action).IsEnd()) {
					this.simulationState.IncreaseSequenceIndex(seq);
					if (action is ADLMoveAction) {
						this.velocity = new Vector2(0, 0);
					}
				}
			}
			else if (action is ADLToStateAction){
				action.PerformAction(this);
				break;
			}
			else if (action is ADLConditionAction){
				ADLConditionAction condition = (ADLConditionAction) action;
				if (condition.PerformAction(this)) {
					this.simulationState.IncreaseSequenceIndex(seq);
				} else {
					this.simulationState.IncreaseSequenceIndex(seq, condition.totalActions + 1);
				}
			}
			else {
				this.simulationState.IncreaseSequenceIndex(seq);
			}
		}
	}

	private void CompileScript() {
		if (this.agentScript == null)
		{
			this.agentScript = ADLScriptFactory.instance.CreateADLScript(this.agentScriptFile.text);
			// Debug.Log("Total SubAgent scripts: " + this.currentScript.subAgentScripts.Count);
			// for (int i = 0; i < this.agentScript.subAgentScripts.Count; i++){
			// 	Debug.Log("Projectile Agent:" + this.currentScript.subAgentScripts[i].agentName);
			// }
			//this.PrintEnemyBehavior();
		}
	}

	private void RecursivePrintParameters(ADLFunction function,int num){

		string tabString = "";
		for (int i = 0; i < num;i++)
		{
			tabString += "\t";
		}
		//Debug.Log (tabString + " Function : " + function.name + " (" + function.parameters.Count + ")");
		for (int i = 0;i < function.parameters.Count;i++)
		{
			//Debug.Log (tabString + "\t" + "Param #" + i + " : ");
			object[] objects = function.parameters[i].Tokens.ToArray();
			for (int j = 0 ; j < objects.Length; j++)
			{
				if (objects[j].ToString() == "ADLFunction")
				{
					RecursivePrintParameters((ADLFunction)objects[j],num+2);
				}
				else
				{
					//Debug.Log(tabString + "\t\tToken #" + j + " : " + objects[j].ToString());
				}
			}
		}
	}

	private void PrintEnemyBehavior() {
		//Debug.Log("Enemy : " + this.currentScript.agentName + " . ") ;
		foreach (ADLState state in this.agentScript.states)
		{
			//Debug.Log("\tState : " + state.name);

			foreach (ADLSequence seq in state.seqs)
			{
				//Debug.Log("\t\tSequence : " + seq.name);

				foreach (ADLAction action in seq.actions)
				{
					if (action.name.Equals("If"))
					{
						//Debug.Log("\t\t\tAction : If , GOTO Action Index #" + action.parameters[0]);
					}
					else
					{
						//Debug.Log("\t\t\tAction : " + action.name);
						foreach (ADLParameter parameter in action.parameters)
						{
							//Debug.Log("\t\t\t\tParam #" + n + " : " + parameter);
							object[] objects = parameter.Tokens.ToArray();
							for (int o = 0 ; o < objects.Length; o++)
							{
								if (objects[o].ToString() == "ADLFunction")
								{
									RecursivePrintParameters((ADLFunction)objects[o],5);
								}
								else
								{
									//Debug.Log("\t\t\t\t\tToken #" + o + " : " + objects[o].ToString());
								}
							}
						}
					}
				}
			}
		}
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
		//this.SetLayer((int) Layer.Invulnerable);
		this.isInvulnerable = true;
		for (int i = 0;i < 10;i++)
		{
			this.toggleSpriteColor();
			yield return new WaitForSeconds(0.25f);
		}
		//this.SetLayer((int) Layer.Enemy);
		this.isInvulnerable = false;
	}

	private void toggleSpriteColor(){
		SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
		if (spriteRenderer.color.Equals(Color.white))
		{
			spriteRenderer.color = Color.black;
		}
		else
		{
			spriteRenderer.color = Color.white;
		}
	}

	public class SimulationState {
		public ADLState currentState;
		internal Dictionary<string, int> sequenceIndexes = new Dictionary<string, int>();
		internal Dictionary<ADLAction, float> elapsedTimes = new Dictionary<ADLAction, float>();
		internal Dictionary<ADLAction, Dictionary<object, object>> singleQueryProperties = new Dictionary<ADLAction, Dictionary<object, object>>();

		public SimulationState(ADLState state) {
			this.SetCurrentState(state);
		}

		public void SetCurrentState(ADLState state) {
			this.currentState = state;
			this.sequenceIndexes.Clear();
			this.elapsedTimes.Clear();
			this.singleQueryProperties.Clear();
			this.currentState.seqs.ForEach((seq) => this.sequenceIndexes.Add(seq.name, 0));
		}

		internal void IncreaseSequenceIndex(ADLSequence seq) {
			this.IncreaseSequenceIndex(seq, 1);
		}

		internal void IncreaseSequenceIndex(ADLSequence seq, int amount) {
			this.elapsedTimes.Remove(seq.actions[this.sequenceIndexes[seq.name]]);
			this.singleQueryProperties.Remove(seq.actions[this.sequenceIndexes[seq.name]]);
			this.sequenceIndexes[seq.name] += amount;
			this.sequenceIndexes[seq.name] %= seq.actions.Count;
		}
	}
}
