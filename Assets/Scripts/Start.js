#pragma strict

// var customGuiStyle : GUIStyle;
// var firstLoad: byte = 1;
private var status: AsyncOperation;

function Start() {
	status = Application.LoadLevelAsync("Flap");
	status.allowSceneActivation = false;
	yield status;
}

function OnGUI () {

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

	//if (GUILayout.Button ("Start Clucking", customGuiStyle)) {
	//	Application.LoadLevel ("Flap");
	//}
	
	/*
	if (firstLoad) {
		firstLoad = 0;
		status = Application.LoadLevelAsync("Flap");
		status.allowSceneActivation = true;
	}
	*/

}

function StartLevel() {
	status.allowSceneActivation = true;
	//Application.LoadLevel("Flap");
}