using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;


public class StartGame : MonoBehaviour {

	private AsyncOperation status;

	private IEnumerator getConfig() {
		WWW www = new WWW("http://cluckbutton.localhost/api/config");
		yield return www;

		if (!string.IsNullOrEmpty (www.text)) {
			Debug.Log(www.text);
			Debug.Log(Json.Deserialize(www.text));
		}
	}

	void Start() {
		StartCoroutine (loadLevel ());
		StartCoroutine (getConfig ());
	}
	
	private IEnumerator loadLevel() {
		status = Application.LoadLevelAsync("Flap");
		status.allowSceneActivation = false;
		yield return status;
	}
	
	void OnGUI () {
		
		if (Input.GetMouseButtonDown(0) || Input.GetKeyUp("space")) {
			StartLevel();
		} else if (Input.touchCount > 0) {
			for(int i = 0; i < Input.touchCount; i++) {
				if (Input.touches[i].phase == TouchPhase.Began) {
					StartLevel();
					break;
				}
			}
		}

	}
	
	void StartLevel() {
		status.allowSceneActivation = true;
		//Application.LoadLevel("Flap");
	}
}
