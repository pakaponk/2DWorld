using System.Collections.Generic;

public class ADLState{
	public string name;
	public List<ADLSequence> seqs;

	public ADLState(string name){
		this.name = name;
		this.seqs = new List<ADLSequence>();
	}
}