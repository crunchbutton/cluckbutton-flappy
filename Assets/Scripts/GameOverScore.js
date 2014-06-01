#pragma strict

var myFont: Font;
private var status: AsyncOperation;

function Start() {
	status = Application.LoadLevelAsync("Flap");
	status.allowSceneActivation = false;
	yield status;
}

function OnGUI () {
	var myStyle = new GUIStyle();
	var score = PlayerPrefs.GetInt("LastScore");
	GUI.color = Color.white;
	myStyle.font = myFont;
	
	//GUILayout.Label(" Score: " + score.ToString(), myStyle);
	
	GUILayout.Label(Screen.dpi.ToString(), myStyle);
}

function Update() {
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
}

function StartLevel() {
	status.allowSceneActivation = true;
}