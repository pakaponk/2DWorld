using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossSelectionButton : MonoBehaviour{
	public string bossName;

	public void OnClicked() {
		StartCoroutine(LoadAsyncScene(this.bossName));
	}

	private IEnumerator LoadAsyncScene(string sceneName) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		while(!asyncLoad.isDone) {
			yield return null;
		}
	}
}
