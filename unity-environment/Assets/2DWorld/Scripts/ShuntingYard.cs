using UnityEngine;
using System.Collections;
using System.Linq;
	
public class ShuntingYard
{
	private Queue token;
	private Queue rpnQueue;
	private readonly string[] OPERATOR_LIST = {"+", "-", "*", "/"};

	public ShuntingYard (Queue token)
	{
		this.token = (Queue)token.Clone();
		buildRPN();
	}

	private void buildRPN(){
		this.rpnQueue = new Queue();
		Stack operatorStack = new Stack();

		object obj;
		while (token.Count != 0)
		{
			obj = token.Dequeue();

			if (obj is int)
			{
				this.rpnQueue.Enqueue((int)obj);
			}
			else if (obj is float)
			{
				this.rpnQueue.Enqueue((float)obj);
			}
			else if (obj is ADLFunction)
			{
				this.rpnQueue.Enqueue((ADLFunction)obj);
			}	
			else if (obj is bool)
			{
				this.rpnQueue.Enqueue((bool)obj);
			}
			else if (obj is string && !OPERATOR_LIST.Contains(obj))
			{
				this.rpnQueue.Enqueue((string)obj);
			}	
			else
			{
				if (operatorStack.Count != 0 && (this.getOperatorPrecedence((string)operatorStack.Peek()) > this.getOperatorPrecedence((string)obj)))
				{
					this.rpnQueue.Enqueue((string)operatorStack.Pop());
				}
				operatorStack.Push((string)obj);
			}
		}

		while(operatorStack.Count != 0)
		{
			this.rpnQueue.Enqueue((string)operatorStack.Pop());
		}
	}

	private int getOperatorPrecedence(string op){
		if (op.Equals("||"))
		{
			return -6;
		}
		else if (op.Equals("&&"))
		{
			return -5;
		}
		else if (op.Equals("==") || op.Equals("!="))
		{
			return -1;
		}
		else if (op.Equals("<") || op.Equals(">") || op.Equals("<=") || op.Equals(">="))
		{
			return 0;
		}
		else if (op.Equals("<<") || op.Equals(">>"))
		{
			return 1;
		}
		else if (op.Equals("+") || op.Equals("-"))
		{
			return 2;
		}
		else if (op.Equals("*") || op.Equals("/") || op.Equals("%"))
		{
			return 3;
		}
		else
		{
			return -9999;
		}
	}

	public void printRPN(){
		object[] objects = this.rpnQueue.ToArray();
		Debug.Log("Print RPN: " + string.Concat(objects));
	}

	public object processRPN(){
		Stack resultStack = new Stack();
		Queue processingQueue = (Queue)this.rpnQueue.Clone();
		while(processingQueue.Count != 0){
			//If Front of Queue is Operand
			//Push Operand into Stack
			if (processingQueue.Peek() is int || 
				processingQueue.Peek() is float || 
				processingQueue.Peek() is bool || 
				processingQueue.Peek() is ADLFunction ||
				processingQueue.Peek() is string && !OPERATOR_LIST.Contains((string)processingQueue.Peek()))
			{
				resultStack.Push(processingQueue.Dequeue());
			}
			//If Front of Queue is Operator
			//Operate top 2 operands then push the result into Stack 
			else
			{
				object op1 = resultStack.Pop();
				object op2 = resultStack.Pop();
				resultStack.Push(operate((string)processingQueue.Dequeue(),CastToFloat(op1),CastToFloat(op2)));
			}
		}

		//Cast the result into appropriate type
		if (resultStack.Peek() is string)
		{
			return resultStack.Pop() as string;
		}
		else if (resultStack.Peek() is bool)
		{
			return (bool)resultStack.Pop();
		}
		else if (resultStack.Peek() is ADLTimesFunction || 
				resultStack.Peek() is ADLIsXFunction || 
				resultStack.Peek() is ADLIsYFunction ||
				resultStack.Peek() is ADLIsCollidedWithFunction)
		{
			return System.Convert.ToBoolean(((ADLFunction)resultStack.Pop()).PerformFunction());
		}
		else
		{
			return this.CastToFloat(resultStack.Pop());;	
		}
	}

	/**
	 * This method is solely used inside ADLMoveAction 
	 */
	public object processRPN(ADLBaseAgent agent, int axis){
		Stack resultStack = new Stack();
		Queue processingQueue = (Queue)this.rpnQueue.Clone();
		while(processingQueue.Count != 0){
			//If Front of Queue is Operand
			//Push Operand into Stack
			if (processingQueue.Peek() is int || 
				processingQueue.Peek() is float || 
				processingQueue.Peek() is bool || 
				processingQueue.Peek() is ADLFunction ||
				processingQueue.Peek() is string && !OPERATOR_LIST.Contains((string)processingQueue.Peek()))
			{
				if (processingQueue.Peek() is int)
				{
					if (axis == 0)
						resultStack.Push((int) agent.horizonDirection * (int) processingQueue.Dequeue());
					else
						resultStack.Push((int) agent.verticalDirection * (int) processingQueue.Dequeue());
				}
				else if (processingQueue.Peek() is float){
					if (axis == 0)
						resultStack.Push((int) agent.horizonDirection * (float) processingQueue.Dequeue());
					else
						resultStack.Push((int) agent.verticalDirection * (float) processingQueue.Dequeue());
				}
				else{
					resultStack.Push(processingQueue.Dequeue());
				}
			}
			//If Front of Queue is Operator
			//Operate top 2 operands then push the result into Stack 
			else
			{
				object op1 = resultStack.Pop();
				object op2 = resultStack.Pop();
				resultStack.Push(operate((string) processingQueue.Dequeue(),CastToFloat(op1, agent, axis),CastToFloat(op2, agent, axis)));
			}
		}

		//Cast the result into appropriate type
		if (resultStack.Peek() is string)
		{
			return resultStack.Pop() as string;
		}
		else if (resultStack.Peek() is bool)
		{
			return (bool)resultStack.Pop();
		}
		else
		{
			return this.CastToFloat(resultStack.Pop(), agent, axis);	
		}
	}

	private float operate(string oparator, float op2, float op1)
	{
		switch(oparator){
			case "+":
				return op1 + op2;
			case "-":
				return op1 - op2;
			case "*":
				return op1 * op2;
			case "/":
				return op1 / op2;
			default: 
				return op1 + op2;
		}
	}

	private float CastToFloat(object obj, ADLBaseAgent agent, int axis)
	{
		if (obj is ADLSinFunction)
		{
			return System.Convert.ToSingle(((ADLSinFunction)obj).PerformFunction());
			// if (axis == 0)
			// 	return System.Convert.ToSingle(((ADLSinFunction)obj).PerformFunction2(agent.horizonDirection > 0));
			// else
			// 	return System.Convert.ToSingle(((ADLSinFunction)obj).PerformFunction2(agent.verticalDirection > 0));
		}
		else if (obj is ADLFunction)
		{
			return System.Convert.ToSingle(((ADLFunction)obj).PerformFunction());
		}
		else
		{	
			return System.Convert.ToSingle(obj);
		}
	}

	private float CastToFloat(object obj)
	{
		if (obj is ADLFunction)
		{
			return System.Convert.ToSingle(((ADLFunction)obj).PerformFunction());
		}
		else
		{
			return System.Convert.ToSingle(obj);
		}
	}
}

