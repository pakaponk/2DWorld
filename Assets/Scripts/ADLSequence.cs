using System.Collections.Generic;

public class ADLSequence{
	public string name;
	public List<ADLAction> actions;

	public ADLSequence(string name){
		this.name = name;
		this.actions = new List<ADLAction>();
	}
}