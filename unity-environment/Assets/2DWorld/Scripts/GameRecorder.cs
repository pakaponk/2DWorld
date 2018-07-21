using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using Firebase;
using Firebase.Storage;

[Serializable]
public class GameRecorder : MonoBehaviour {
	public static GameRecorder instance;
	
	//Recorded Info
	public int seed;
	[NonSerialized]
	public System.Random random;
	private GameState state;
	[SerializeField]
	private List<PlayerInput> playerInputSequence = new List<PlayerInput>();
	private List<List<Entity>> entitiesHistory = new List<List<Entity>>();
	public enum GameState: int { Running = 1, End = 2};
	public string bossName;
	public int round;
	
	private Text alertText;
	public bool isControlledByPlayer = true;
	private string playerInputRecord;

	//Firebase
	private const string CLOUD_STORAGE_URL = "gs://sample-e1209.appspot.com";
	private StorageReference storage_ref;
	private bool isFileUploaded = false;

	// Use this for initialization
	void Awake() {
	
	}
	
	void Start () {
		instance = this;

		this.alertText = GameObject.Find("AlertText").GetComponent<Text>();

		if (!isControlledByPlayer) {
			this.playerInputRecord = File.ReadAllText("Records/ShadowMan/Record14.json");
			GameRecord info = JsonUtility.FromJson<GameRecord>(playerInputRecord); 
			this.seed = info.seed;
			this.playerInputSequence = info.playerInputSequence;
			
		} else {
			System.Random rnd = new System.Random();
			this.seed = rnd.Next(100);
		}
		
		this.state = GameState.Running;

		UnityEngine.Random.InitState(this.seed);
		this.random = new System.Random(GameRecorder.instance.seed);

		alertText.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (state.Equals(GameState.Running) && isControlledByPlayer) {
			PlayerInput input = new PlayerInput(ADLBaseAgent.FindAgent("Player"),Input.GetAxis("Horizontal"), Input.GetButtonDown("Fire1"), Input.GetButtonDown("Jump"), Input.GetButtonUp("Jump"));
			playerInputSequence.Add(input);

			// string json = JsonUtility.ToJson(new ListWrapper<Entity>(entities));
			// entitiesRecord.Add(json.Substring(8, json.Length - 9));
		}

		if (state.Equals(GameState.End)) {
			if (this.isFileUploaded) {
				alertText.text = "Press Enter to Play again";
			} else {
				alertText.text = "Uploading Report ...";
			}
		}
	}

	void FixedUpdate () {
		if (state.Equals(GameState.Running) && isControlledByPlayer) {
			// PlayerInput input = new PlayerInput(ADLBaseAgent.FindAgent("Player"),Input.GetAxis("Horizontal"), Input.GetButtonDown("Fire1"), Input.GetButtonDown("Jump"), Input.GetButtonUp("Jump"));
			// playerInputSequence.Add(input);

			List<Entity> entities = new List<Entity>();
			foreach (ADLBaseAgent agent in ADLBaseAgent.agents) {
				Entity entity = new Entity(agent);
				entities.Add(entity);
			}
			entitiesHistory.Add(entities);
		}
	}

	public void End() {
		this.state = GameState.End;

		if (isControlledByPlayer) {
			CreatePlayerRecordFile();	
		} else {
			alertText.gameObject.SetActive(true);
			alertText.text = "End";
		}
	}

	public void RemoveCurrentPlayerInput() {
		if (this.playerInputSequence.Count > 0)
			this.playerInputSequence.RemoveAt(0);
	}

	public PlayerInput GetCurrentPlayerInput() {
		if (this.playerInputSequence.Count > 0)
			return this.playerInputSequence[0];
		else
			return new PlayerInput(null, 0, false, false, false);
	}

	private void CreatePlayerRecordFile() {
		string fileName = this.GenerateRecordFileName();
		while (File.Exists(fileName)) {
			this.round++;
			fileName = this.GenerateRecordFileName();
		}
		string json = JsonUtility.ToJson(this);
		
		Directory.CreateDirectory(@"Records/" + this.bossName);
		System.IO.File.WriteAllText(fileName, json);

		FirebaseApp app = FirebaseApp.Create();
		FirebaseStorage storage = FirebaseStorage.GetInstance(CLOUD_STORAGE_URL);
		this.storage_ref = storage.GetReferenceFromUrl(CLOUD_STORAGE_URL);
		
		StorageReference record_ref = this.storage_ref.Child(fileName);
		record_ref.PutBytesAsync(Encoding.UTF8.GetBytes(json))
			.ContinueWith (task => {
				if (task.IsFaulted || task.IsCanceled) {
					Debug.Log(task.Exception.ToString());
					// Uh-oh, an error occurred!
				} else {
					// Metadata contains file metadata such as size, content-type, and download URL.
					Firebase.Storage.StorageMetadata metadata = task.Result;
					string download_url = metadata.DownloadUrl.ToString();
					Debug.Log("Finished uploading...");
					Debug.Log("download url = " + download_url);

					this.isFileUploaded = true;
				}
			});
		alertText.gameObject.SetActive(true);
	}

	private string GenerateRecordFileName() {
		return @"Records/" + this.bossName + @"/Record" + this.round + ".json";
	}

	public void SetGameState(GameState state) {
		if (state.Equals(GameState.End)) {

		}
		else {

		}
		this.state = state;
	}

	[Serializable]
	public class Entity{
		public float x;
		public float y;
		public float width;
		public float height;
		public string agentName; 
		public float lifePoint;
		public float attack;
		public bool isAttacker;
		public bool isDefender;
		public bool isFlippable;
		public bool isFlipper;
		public bool isProjectile;
		public bool isInvulnerable;
		public bool isHittableByProjectile;
		public bool isHittableByEnvironment;
		public string group; // Is it on player side or enemy side
		public int horizonDirection;
		public int verticalDirection;
		public string currentState;
		
		public Entity(ADLBaseAgent agent) {
			this.x = agent.transform.position.x;
			this.y = agent.transform.position.y;
			
			if (agent is ADLEnvironment) {
				this.width = 0;
				this.height = 0;
			} else {
				this.width = agent.transform.localScale.x * agent.GetComponent<BoxCollider2D>().size.x;
				this.height = agent.transform.localScale.y * agent.GetComponent<BoxCollider2D>().size.y;
			}

			this.agentName = agent.agentName;
			this.lifePoint = agent.lifePoint;
			this.attack = agent.attack;
			this.isAttacker = agent.isAttacker;
			this.isDefender = agent.isDefender;
			this.isFlippable = agent.isFlippable;
			this.isFlipper = agent.isFlipper;
			this.isProjectile = agent.isProjectile;
			this.isInvulnerable = agent.isInvulnerable;
			this.isHittableByProjectile = agent.isHittableByProjectile;
			this.isHittableByEnvironment = agent.isHittableByEnvironment;
			this.group = agent.group.ToString();
			this.horizonDirection = (int) agent.horizonDirection;
			this.verticalDirection = (int) agent.verticalDirection;
			
			if (agent is ADLAgent) {
				this.currentState = ((ADLAgent) agent).simulationState.currentState.name;
			}
		}
	}

	public class ListWrapper<T> {
		public List<T> list;
		public ListWrapper(List<T> list) {
			this.list = list;
		}
	}

	public class GameRecord {
		public int seed;
		public List<PlayerInput> playerInputSequence = new List<PlayerInput>();
		public string bossName;
		public int round;
	}
}
