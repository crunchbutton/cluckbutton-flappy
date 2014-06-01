#pragma strict

var customGuiStyle : GUIStyle;

private var score : int = 0;
private var scoreToSkip : int = 1;

static var obstacleSpeed : float = 1.5f;
static var obstacleSpeedOffset : float = 1f;
static var speed : double = .1;

function Awake() {
	InvokeRepeating("UpdateScore", obstacleSpeedOffset, obstacleSpeed);
	
	if (Screen.dpi >= 320) {
		speed = .2;
	}
}

function OnDestroy () {
	PlayerPrefs.SetInt("LastScore",getScore());
}

function OnGUI () {
	GUILayout.Label("Score: " + getScore().ToString(), customGuiStyle);
}


function UpdateScore() {
	score++;
}

function getScore() {
	return score - scoreToSkip > 1 ? score - scoreToSkip : 0;
}