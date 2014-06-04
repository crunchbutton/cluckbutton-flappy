using UnityEngine;
using System.Collections;

public class GameOverScore : MonoBehaviour {
	
	private AsyncOperation status;

	IEnumerator Start() {
		status = Application.LoadLevelAsync("Flap");
		status.allowSceneActivation = false;
		yield return status;
	}
	
	void loginComplete () {
		Debug.Log("logged in");
	}
	
	void loginComplete(FBResult result) {
		if (result.Error != null) {
			Debug.Log(result.Error);
		} else if (!FB.IsLoggedIn) {
			Debug.Log("canceled");
		} else {
			Debug.Log("success");
		}
	}
	
	void initComplete () {
		Debug.Log("init complete in");
	}
	
	void init() {
		FB.Init(initComplete);
	}
	
	void login() {
		FB.Login("email,publish_actions", loginComplete);
	}
	
	void logout() {
		FB.Logout();
	}

	void OnGUI() {

		var myStyle = new GUIStyle();
		var score = PlayerPrefs.GetInt("LastScore");
		GUI.color = Color.white;
		
		GUILayout.Label(" Score: " + score.ToString(), myStyle);
		
		if (GUI.Button(new Rect(10,20,100,60),"Init")) {
			init ();
			
		}
		
		if (GUI.Button(new Rect(10,100,100,60),"Login")) {
			login ();
		}
		
		if (GUI.Button(new Rect(10,180,100,60),"Logout")) {
			logout ();
		}
	}

	void Update() {
		/*
	if (Input.GetMouseButtonDown(0) || Input.GetKeyUp("space")) {
		StartLevel();
	} else if (Input.touchCount > 0) {
		for(var i : int = 0; i < Input.touchCount; i++) {
			if (Input.touches[i].phase == TouchPhase.Began) {
				StartLevel();
				break;
			}
		}
	}
	*/
	}
	
	void StartLevel() {
		status.allowSceneActivation = true;
	}
	
}
