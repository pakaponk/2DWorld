using System.Collections.Generic;
using System;

public class ADLParameter {
	private readonly Queue<ADLToken> tokens;
	private Queue<ADLToken> rpnQueue;

	public Queue<ADLToken> Tokens {get{return tokens;}}

	public ADLParameter() {
		this.tokens = new Queue<ADLToken>();
	}

	private void CreateRPN() {
		Queue<ADLToken> tokens = new Queue<ADLToken>(this.tokens);
        
        this.rpnQueue = new Queue<ADLToken>();
		Stack<ADLToken> operatorStack = new Stack<ADLToken>();

		while(tokens.Count > 0) {
			ADLToken token = tokens.Dequeue(); 

            if (token.IsOperator()) {
                if (operatorStack.Count > 0 &&
                    GetOperatorPrecedence(operatorStack.Peek().Value as string) > GetOperatorPrecedence(token.Value as string)) {
                    this.rpnQueue.Enqueue(operatorStack.Pop());
                }
                operatorStack.Push(token);
            } else {
                this.rpnQueue.Enqueue(token);
            }
		}

        while(operatorStack.Count > 0) {
			this.rpnQueue.Enqueue(operatorStack.Pop());
		}
	}

    public object ProcessRPN() {
        if (this.rpnQueue == null) {
            CreateRPN();
        }

        Stack<ADLToken> resultStack = new Stack<ADLToken>();
        Queue<ADLToken> processQueue = new Queue<ADLToken>(this.rpnQueue);

        while(processQueue.Count > 0){
            //If Front of Queue is Operator
			//Operate top 2 operands then push the result into Stack 
            if (processQueue.Peek().IsOperator()) {
                ADLToken operand2 = resultStack.Pop();
                ADLToken operand1 = resultStack.Pop();

                string opt = processQueue.Dequeue().Value as string;
                resultStack.Push(Operate(opt, operand1, operand2));
            }
            //If Front of Queue is Operand
			//Push Operand into Stack
			else
			{
				resultStack.Push(processQueue.Dequeue());
			}
		}

        //Cast the result into appropriate type
        ADLToken token = resultStack.Pop();
		if (token.Value is string)
		{
			return token.Value as string;
		}
		else if (token.Value is bool)
		{
			return token.Value as bool?;
		}
		else if (token.Value is ADLTimesFunction || 
				token.Value is ADLIsXFunction || 
				token.Value is ADLIsYFunction ||
				token.Value is ADLIsCollidedWithFunction)
		{
			return System.Convert.ToBoolean((token.Value as ADLFunction).PerformFunction());
		}
        else if (token.Value is ADLRandomFunction) {
            return ((ADLRandomFunction) token.Value).PerformFunction();
        }
		else
		{
			return token.ToFloat();	
		}
    }

    private ADLToken Operate(string opt, ADLToken operand1, ADLToken operand2) {
        switch (opt) {
            case "+":
            case "-":
            case "*":
            case "/":
                return new ADLToken(ArithmeticOperate(opt, operand1.ToFloat(), operand2.ToFloat()));
            case "<":
            case ">":
            case "<=":
            case ">=":
            case "==":
            case "!=":
                return new ADLToken(ComparisonOperate(opt, operand1.ToFloat(), operand2.ToFloat()));
            case "||":
            case "&&":
                return new ADLToken(LogicalOperate(opt, operand1.ToBoolean(), operand2.ToBoolean()));
            default:
                throw new Exception("Unknown Operator");
        }
    }

    private float ArithmeticOperate(string opt, float opd1, float opd2) {
        switch(opt){
			case "+":
				return opd1 + opd2;
			case "-":
				return opd1 - opd2;
			case "*":
				return opd1 * opd2;
			case "/":
				return opd1 / opd2;
			default: 
				throw new Exception("Unknown Operator");
		}
    }

    private bool ComparisonOperate(string opt, float opd1, float opd2) {
        switch(opt) {
            case "<":
                return opd1 < opd2;
            case ">":
                return opd1 > opd2;
            case "<=":
                return opd1 <= opd2;
            case ">=":
                return opd1 >= opd2;
            case "==":
                return opd1 == opd2;
            case "!=":
                return opd1 != opd2;
            default:
                throw new Exception("Unknown Operator");
        }
    }

    private bool LogicalOperate(string opt, bool opd1, bool opd2) {
        switch(opt) {
            case "||":
                return opd1 || opd2;
            case "&&":
                return opd1 && opd2;
            default:
                throw new Exception("Unknown Operator");
        }
    }

	private int GetOperatorPrecedence(string opt){
        switch (opt){
            case "||":
                return -6;
            case "&&":
                return -5;
            case "==":
            case "!=":
                return -1;
            case "<":
            case ">":
            case "<=":
            case ">=":
                return 0;
            case "<<":
            case ">>":
                return 1;
            case "+":
            case "-":
                return 2;
            case "*":
            case "/":
            case "%":
                return 3;
            default:
                return -9999;
        }
    }
}
