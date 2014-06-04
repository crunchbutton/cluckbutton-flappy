using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	public GUIStyle scoreStyle;
	public GUIStyle speedStyle;
	
	private int score = 0;
	private int scoreToSkip = 2;
	private double sourceSpeed = .1;
	private int incrimentAt = 10;
	
	public static float obstacleSpeed = 1.5f;
	public static float obstacleSpeedOffset = 1f;
	public static double speed = .1;
	
	void Awake() {
		InvokeRepeating("UpdateScore", obstacleSpeedOffset, obstacleSpeed);
		
		if (Screen.dpi >= 320) {
			speed = sourceSpeed = .2;
		}
	}
	
	void OnDestroy () {
		PlayerPrefs.SetInt("LastScore", getScore());
	}
	
	void OnGUI () {
		GUILayout.Label("Score: " + getScore().ToString(), scoreStyle);
		GUILayout.Label("Speed: " + speed.ToString(), speedStyle);
	}
	
	
	void UpdateScore() {
		score++;
		speed = sourceSpeed + (getScore() / incrimentAt) * .01;
	}
	
	int getScore() {
		return score > scoreToSkip ? score - scoreToSkip : 0;
	}
}