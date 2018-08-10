using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MLAgents;

public class BossFightAcademy : Academy {
    public Vector3[] playerInitialPositions;
    public TextAsset[] enemyScriptFiles;

    public static BossFightAcademy Instance { 
        get {
            return instance;
        }
    }

    [HideInInspector]
    public Vector3 InitialPosition { get { return initialPosition; } }

    private Vector3 initialPosition;

    [HideInInspector]
    public ADLScript EnemyScript { get { return enemyScript; } }
    private ADLScript enemyScript;

    private static BossFightAcademy instance;

    private System.Random random;

    private List<ADLScript> scripts;

    void Start() {
        instance = this;
        
        this.random = new System.Random();
        //this.scripts = new Dictionary<int, ADLScript>();
        this.scripts = enemyScriptFiles
            .ToList()
            .ConvertAll<ADLScript>(scriptFile => ADLScriptFactory.instance.CreateADLScript(scriptFile.text));

    }

    public override void AcademyReset() {
        // initialPosition = RandomInitialPosition();
        // enemyScript = RandomEnemyScript();
    }

    public Vector3 RandomInitialPosition() {
        int size = playerInitialPositions.Length;
        return playerInitialPositions[random.Next(size)];
    }

    public ADLScript RandomEnemyScript() {
        int size = enemyScriptFiles.Length;
        int index = random.Next(size);

        // if (!this.scripts.ContainsKey(index)) {
        //     TextAsset enemyScriptFile = enemyScriptFiles[index];
        //     this.scripts.Add(index, ADLScriptFactory.instance.CreateADLScript(enemyScriptFile.text));
        // }

        return this.scripts[index];
    }
}
