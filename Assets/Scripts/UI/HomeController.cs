using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnLoginButtonClicked () {
		StartCoroutine(LoadAsyncScene("BossSelection"));
	}

	private IEnumerator LoadAsyncScene(string sceneName) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		while(!asyncLoad.isDone) {
			yield return null;
		}
	}
}
