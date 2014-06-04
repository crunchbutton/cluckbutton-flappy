using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	private AsyncOperation status;
	
	IEnumerator Start() {
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
