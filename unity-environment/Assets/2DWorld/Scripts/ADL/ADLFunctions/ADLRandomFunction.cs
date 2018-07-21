public class ADLRandomFunction: ADLFunction{
    public ADLRandomFunction(string name): base(name) {
        
    }

    public override object PerformFunction() {
        int totalParams = this.parameters.Count;
        switch (totalParams) {
            case 0:
                return UnityEngine.Random.Range(0.00f, 1.00f);
            default:
                System.Random rnd = GameRecorder.instance.random;
                return this.parameters[rnd.Next(totalParams)].ProcessRPN();
        }
    }
}