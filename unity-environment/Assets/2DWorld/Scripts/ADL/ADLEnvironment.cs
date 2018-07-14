public class ADLEnvironment : ADLBaseAgent {
    public new void Start()
    {
        this.gameObject.layer = (int) Layer.Environment;
        this.agentName = this.gameObject.name;
    }
}