using System.Linq;

public class ADLToken {

    private static readonly string[] OPERATOR_LIST = {"+", "-", "*", "/", ">", "<", ">=", "<=", "==", "!=", "||", "&&"};

    private readonly object token;

    public object Value {get{ return token;}}

    public ADLToken(object token) {
        this.token = token;
    }

    public bool IsOperator() {
        return (token is string) && OPERATOR_LIST.Contains(token as string);
    }

    public float ToFloat() {
        if (token is ADLFunction) {
            return System.Convert.ToSingle((token as ADLFunction).PerformFunction());
        } else {
            return System.Convert.ToSingle(token);
        }
    }

    public bool ToBoolean() {
        if (token is ADLFunction) {
            return System.Convert.ToBoolean((token as ADLFunction).PerformFunction());
        } else {
            return System.Convert.ToBoolean(token);
        }
    }
}
