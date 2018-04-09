using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ADLScriptFactory{
    public static ADLScriptFactory instance = new ADLScriptFactory();

    private const string NamePattern = @"\.(?<Name>[A-Z][\w-]*)\{";
	private const string StatePattern =  @"\.(?<State>[\w]+)\{|(?<EndName>\})";
	private const string SequencePattern = @"\.(?<Sequence>[\w]+)\{|(?<EndState>\})";
	private const string ActionPattern = @"(?<Action>[A-Z][\w]+)\(|(?<EndSequence>\})|(?<If>if|else if)\s+\(|(?<Else>else)\{";
	private const string ParameterPattern = @"'(?<String>[^']*)'|""(?<String2>[^""]*)""|(?<Double>\-?[\d]+\.[\d]+)|(?<Pi>PI)|(?<Int>\-?[\d]+)|(?<Comma>,)|(?<EndAction>\)\;)|(?<Function>[A-Z][\w]*)\(|(?<EndIfBracket>\)\s*\{)|(?<EndBracket>\))|(?<OrOperator>\|\|)|(?<AndOperator>\&\&)|(?<PlusOperator>\+)|(?<MinusOpaerator>\-)|(?<MultiplyOperator>\*)|(?<DivideOperator>\/)|(?<ModOperator>\%)|(?<AndOperator>\&\&)|(?<OrOperator>\|\|)|(?<StartBracket>\()|(?<Boolean_True>true)|(?<Boolean_False>false)|(?<MoreThanOrEqual>\>\=)|(?<LessThanOrEqual>\<\=)|(?<MoreThan>\>)|(?<LessThan>\<)|(?<Equal>\=\=)|(?<NotEqual>\!\=)";
    private const string CommentPattern = @"\/\/.*|\/\*[\w\W]*?\*\/";

	private static readonly Regex RegexNamePattern = new Regex(NamePattern);
	private static readonly Regex RegexStatePattern = new Regex(StatePattern);
	private static readonly Regex RegexSequencePattern = new Regex(SequencePattern);
	private static readonly Regex RegexActionPattern = new Regex(ActionPattern);
	private static readonly Regex RegexParameterPattern = new Regex(ParameterPattern);
    private static readonly Regex RegexCommentPattern = new Regex(CommentPattern);

    private ADLScript primaryAgentScript;

    private ADLScript currentAgentScript;
	private ADLState currentState;
	private ADLSequence currentSequence;
	private ADLAction currentAction;
	private ADLFunction currentFunction;
    private ADLParameter currentParameter;

    private Stack<String> compilerStack = new Stack<String>();
    private Stack<ADLConditionAction> conditionStack = new Stack<ADLConditionAction>();
    private Stack<ADLFunction> functionStack = new Stack<ADLFunction>();

    private const bool IS_DEBUGGING = false;   

    public ADLScript CreateADLScript(string scriptText){
        scriptText = RegexCommentPattern.Replace(scriptText, "");

        this.primaryAgentScript = null;
        
        Regex currentRegex = GetRegexByState(null);
		int currentADLScriptIndex = 0; 
		string parsingState = "name";
		string nextParsingState;
		Match token;
		
		while (currentRegex.IsMatch(scriptText, currentADLScriptIndex))
		{
			token = currentRegex.Match(scriptText, currentADLScriptIndex);
			nextParsingState = this.Parse(parsingState, token);

			if (nextParsingState == null || !nextParsingState.Equals(parsingState))
			{
				currentRegex = GetRegexByState(nextParsingState);
				parsingState = nextParsingState;
			}
            currentADLScriptIndex = token.Index + token.Length;
		}

        return this.primaryAgentScript;
    }

	private string Parse(string parsingState, Match token){
		string nextParsingState = null;	
		switch(parsingState)
		{
			case "state":
				nextParsingState = this.ParseLevelState(parsingState, token);
				break;
            case "sequence":
                nextParsingState = this.ParseLevelSequence(parsingState, token);
                break;
            case "action":
                nextParsingState = this.ParseLevelAction(parsingState, token);
                break;
            case "parameter":
                nextParsingState = this.ParseLevelParameter(parsingState, token);
                break;
			default:
				nextParsingState = this.ParseLevelName(parsingState, token);
				break;
		}
		return nextParsingState;
	}

	private Regex GetRegexByState(string state){
		switch(state)
		{
			case "state":
				return RegexStatePattern;
			case "sequence":
				return RegexSequencePattern;
			case "action":
				return RegexActionPattern;
			case "parameter":
				return RegexParameterPattern;
			default:
				return RegexNamePattern;
		}
	}

	private string ParseLevelName(string parsingState, Match token){
		if (token.Groups["Name"].Length != 0)
		{
            this.compilerStack.Push(parsingState);
			parsingState = "state";
            
            ADLScript agentScript = new ADLScript(token.Groups["Name"].Value);
            this.currentAgentScript = agentScript;

			if (this.primaryAgentScript == null)
			{
				this.Log("Agent Name : " + token.Groups["Name"]);
                this.primaryAgentScript = agentScript;
			}
			else
			{
				this.Log("SubAgent Name : " + token.Groups["Name"]);
				this.primaryAgentScript.subAgentScripts.Add(agentScript);
			}
		}
		return parsingState;
	}

	private string ParseLevelState(string parsingState, Match token){
		if (token.Groups["EndName"].Length != 0)
		{
			parsingState = this.compilerStack.Pop();
		}
		else if (token.Groups["State"].Length != 0)
		{

            ADLState state = new ADLState(token.Groups["State"].Value);
			this.currentState = state;
			this.currentAgentScript.states.Add(state);

            this.compilerStack.Push(parsingState);
			if (token.Groups["State"].Value.Equals("init") || token.Groups["State"].Value.Equals("des"))
			{
                parsingState = "action";
				
                this.Log("Special State : " + token.Groups["State"]);
				
                ADLSequence sequence = new ADLSequence("seq");
                this.currentSequence = sequence;
				this.currentState.seqs.Add(sequence);
			}
			else
			{
				parsingState = "sequence";
				this.Log("State : " + token.Groups["State"]);
			}
		}
		return parsingState;
	}

    private string ParseLevelSequence(string parsingState, Match token){
        if (token.Groups["EndState"].Length != 0)
        {	
            parsingState = this.compilerStack.Pop();
        }
        else if (token.Groups["Sequence"].Length != 0)
        {
            this.compilerStack.Push(parsingState);
            parsingState = "action";

            this.Log("Sequence : " + token.Groups["Sequence"]);
            
            ADLSequence sequence = new ADLSequence(token.Groups["Sequence"].Value);
            this.currentSequence = sequence;
            this.currentState.seqs.Add(sequence);
        }
        return parsingState;
    }

    private string ParseLevelAction(string parsingState, Match token)
    {
        if (token.Groups["EndSequence"].Length != 0)
        {	
            if (this.conditionStack.Count > 0)
            {
                ADLConditionAction condition = this.conditionStack.Pop();
                condition.totalActions = this.currentSequence.actions.Count - this.currentSequence.actions.IndexOf(condition) - 1;
                this.Log("End Condition");
            }
            else
            {
                parsingState = this.compilerStack.Pop();
            }
        }
        else if (token.Groups["Action"].Length != 0)
        {
            this.compilerStack.Push(parsingState);
            parsingState = "parameter";

            this.Log("Action : " + token.Groups["Action"]);
            
            ADLAction action = this.createNewADLAction(token.Groups["Action"].Value);
            ADLParameter parameter = new ADLParameter();
            this.currentAction = action;
            this.currentParameter = parameter;
            this.currentAction.parameters.Add(parameter);
            this.currentSequence.actions.Add(action);
        }
        else if (token.Groups["If"].Length != 0)
        {
            this.compilerStack.Push(parsingState);
            parsingState = "parameter";

            ADLConditionAction action = new ADLConditionAction(token.Groups["Action"].Value);
            ADLParameter parameter = new ADLParameter();
            this.currentAction = action;
            this.currentParameter = parameter;
            this.currentAction.parameters.Add(parameter);
            this.currentSequence.actions.Add(action);

            this.conditionStack.Push(action);
        }
        else if (token.Groups["Else"].Length != 0)
        {

        }
        return parsingState;
    }

    private string ParseLevelParameter(string parsingState, Match token){
        if (token.Groups["EndAction"].Length != 0 ||
            token.Groups["EndIfBracket"].Length != 0)
        {
            parsingState = this.compilerStack.Pop();

            //Remove the first parameter from action if its number of tokens is zero
            if (this.currentAction.parameters.Count == 1 &&
                this.currentParameter.Tokens.Count == 0) {
                this.currentAction.parameters.Remove(this.currentParameter);
            }
        }
        else if (token.Groups["EndBracket"].Length != 0)
        {
            //The token is the ) of an ADLFunction
            if (this.functionStack.Count > 0)
            {
                //Remove the first parameter from function if its number of tokens is zero
                if (this.currentFunction.parameters.Count == 1 &&
                    this.currentParameter.Tokens.Count == 0) {
                    this.currentFunction.parameters.Remove(this.currentParameter);
                }

                functionStack.Pop ();
                if (functionStack.Count > 0) {
                    this.currentFunction = functionStack.Peek();
                    this.currentParameter = this.currentFunction.parameters[this.currentFunction.parameters.Count - 1];
                } 
                else {
                    this.currentFunction = null;
                    this.currentParameter = this.currentAction.parameters[this.currentAction.parameters.Count - 1];
                }
            }
        }
        else if (token.Groups["StartBracket"].Length != 0)
        {

        }
        else if (token.Groups["Function"].Length != 0)
        {
            this.Log("Function : " + token.Groups["Function"].Value);

            //Create new Function and push it to Function Stack
            //To tell the compiler that the next parameter found in this state will be a parameter of the recently generated function  
            ADLFunction function = this.createNewADLFunction(token.Groups["Function"].Value);
            ADLParameter parameter = new ADLParameter();
            
            this.currentParameter.Tokens.Enqueue(new ADLToken(function));
            this.currentFunction = function;
            this.currentParameter = parameter;
            this.currentFunction.parameters.Add(parameter);
            this.functionStack.Push(function);
        }
        else if (token.Groups["Comma"].Length == 0)
        {
            if (token.Groups["Double"].Length != 0)
            {
                this.currentParameter.Tokens.Enqueue(new ADLToken(float.Parse(token.Value)));
            }
            else if (token.Groups["Int"].Length != 0)
            {
                this.currentParameter.Tokens.Enqueue(new ADLToken(int.Parse(token.Value)));
            }
            else if (token.Groups["Pi"].Length != 0)
            {
                this.currentParameter.Tokens.Enqueue(new ADLToken(System.Convert.ToSingle(Math.PI)));
            }
            else if (token.Groups["Boolean_True"].Length != 0 || token.Groups["Boolean_False"].Length != 0 )
            {
                this.currentParameter.Tokens.Enqueue(new ADLToken(bool.Parse(token.Value)));
            }
            else
            {
                this.currentParameter.Tokens.Enqueue(new ADLToken(token.Value));
            }
        }
        else if (token.Groups["Comma"].Length > 0)
        {
            ADLParameter paramater = new ADLParameter();
            if (functionStack.Count == 0) {
                this.currentAction.parameters.Add(paramater);
            }
            else {
                this.currentFunction.parameters.Add(paramater);
            }
            this.currentParameter = paramater;
        }
        return parsingState;
    }

    private ADLAction createNewADLAction(string name){

		ADLAction newAction;

		switch(name)
		{
			case "Move":
				newAction = new ADLMoveAction(name); 
				break;
            case "MoveToward":
                newAction = new ADLMoveTowardAction(name); 
                break;
			case "Set":
				newAction = new ADLVarSetAction(name);
				break;
            case "Spawn" :
                newAction = new ADLSpawnAction(name);
                break;
            case "Wait" : 
                newAction = new ADLWaitAction(name);
                break;
            case "FlipX" : 
                newAction = new ADLFlipXAction(name);
                break;
            case "FlipY" : 
                newAction = new ADLFlipYAction(name);
                break;
            case "ToState" :
                newAction = new ADLToStateAction(name);
                break;
            case "FlipTowardPlayer" :
                newAction = new ADLFlipTowardPlayerAction(name);
                break;
            case "Destroy" :
                newAction = new ADLDestroyAction(name);
                break;
			default:
				throw new Exception("Unable to create Unknown Action");
		}

		return newAction;
	}

	private ADLFunction createNewADLFunction(string name){

		ADLFunction adlFunction;

		switch(name)
		{
			case "Linear":
				adlFunction = new ADLLinearFunction(name);
				break;
			case "Sin":
				adlFunction = new ADLSinFunction(name);
				break;
			case "Cos":
				adlFunction = new ADLCosFunction(name);
				break;
			case "SquareWave":
				adlFunction = new ADLSquareWaveFunction(name);
				break;
            case "Times" : 
                adlFunction = new ADLTimesFunction(name);
                break;
            case "IsX" : 
                adlFunction = new ADLIsXFunction(name);
                break;
            case "IsY" : 
                adlFunction = new ADLIsYFunction(name);
                break;
            case "IsCollidedWith":
                adlFunction = new ADLIsCollidedWithFunction(name);
                break;
            case "GetX":
                adlFunction = new ADLGetXFunction(name);
                break;
            case "GetY":
                adlFunction = new ADLGetYFunction(name);
                break;
            case "GetCenterDistance":
                adlFunction = new ADLGetCenterDistanceFunction(name);
                break;
            case "Random":
                adlFunction = new ADLRandomFunction(name);
                break;
			default:
				adlFunction = null;
				break;
		}

		return adlFunction;
	
	}

    private void Log(string message){
        if (IS_DEBUGGING){
            Debug.Log(message);
        }
    }
}