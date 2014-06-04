#pragma strict

public var scoreStyle : GUIStyle;
public var speedStyle : GUIStyle;

private var score : int = 0;
private var scoreToSkip : int = 2;
private var sourceSpeed : double = .1;

static var obstacleSpeed : float = 1.5f;
static var obstacleSpeedOffset : float = 1f;
static var speed : double = .1;



function Awake() {
	InvokeRepeating("UpdateScore", obstacleSpeedOffset, obstacleSpeed);
	
	if (Screen.dpi >= 320) {
		speed = sourceSpeed = .2;
	}
	
	var www : WWW = new WWW ("https://crunchbutton.com/api/config");
	yield www;
	
	if (www.error == null) {
		
	} else {
		Debug.Log(www.error);
	}
}

function OnDestroy () {
	PlayerPrefs.SetInt("LastScore",getScore());
}

function OnGUI () {
	GUILayout.Label("Score: " + getScore().ToString(), scoreStyle);
	GUILayout.Label("Speed: " + speed.ToString(), speedStyle);
}


function UpdateScore() {
	score++;
	// incriment at 10
	speed = sourceSpeed + (getScore() / 10) * .01;
}

function getScore() {
	return score > scoreToSkip ? score - scoreToSkip : 0;
}
