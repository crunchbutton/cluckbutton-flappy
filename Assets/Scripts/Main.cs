using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;

public class Main : MonoBehaviour {

	public GUIStyle scoreStyle;
	public GUIStyle logoStyle;
	public GameObject[] obstacles;

	public Player playerObject;
	
	private int score = 0;
	private int scoreToSkip = 2;
	private double sourceSpeed = .1;
	private int incrimentAt = 10;
	private bool hasPlayed = false;

	public static GameObject BGM;
	public static float obstacleSpeed = 1.5f;
	public static float obstacleSpeedOffset = 1f;
	public static double speed = .1;

	private Vector3 playerStart;




	public static bool isPlaying = false;

	void Update() {
		if (!isPlaying) {

			//return;
		} else {

		}

		// move the camera
		double newSpeed = Camera.main.transform.position.x + speed;
		Camera.main.transform.position = new Vector3((float)newSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);

		// move the player
		if (!playerObject.isDying) {
			playerObject.transform.position = new Vector3((float)(playerObject.transform.position.x + speed), playerObject.transform.position.y, playerObject.transform.position.z);
		}
	}
	
	void Awake() {
		playerStart = playerObject.transform.position;
		Debug.Log (playerStart.x);
		if (Screen.dpi >= 320) {
			speed = sourceSpeed = .2;
		}
		BGM = GameObject.Find("BGM");
		StartCoroutine (getConfig ());

		reset (true);
	}
	
	void OnDestroy () {
		//reset ();
	}
	
	void OnGUI () {
		GUILayout.Label("Score: " + getScore().ToString(), scoreStyle);

		if (!isPlaying) {
			if (hasPlayed) {
				GUILayout.Label("Game Over" + speed.ToString(), logoStyle);
			} else {
				GUILayout.Label("Cluckbutton" + speed.ToString(), logoStyle);
			}

			if (GUI.Button(new Rect(10,80,100,60),"start")) {
				play ();
			}
		}
	}

	void UpdateScore() {
		score++;
		speed = sourceSpeed + (getScore() / incrimentAt) * .01;
	}
	
	int getScore() {
		return score > scoreToSkip ? score - scoreToSkip : 0;
	}

	// call to play
	void play() {
		if (isPlaying) {
			return;
		}
		hasPlayed = true;
		isPlaying = true;
		InvokeRepeating("UpdateScore", obstacleSpeedOffset, obstacleSpeed);
		InvokeRepeating("CreateObstacle", obstacleSpeedOffset, obstacleSpeed);
		CancelInvoke ("FakeFly");
		BGM.audio.Play();

		playerObject.rigidbody2D.gravityScale = 1;

	}

	// call after dying
	public void reset(bool force = false) {
		if (!isPlaying && !force) {
			return;
		}

		playerObject.rigidbody2D.gravityScale = .02f;

		InvokeRepeating("FakeFly", 2f, 4.1f);

		BGM.audio.Stop ();
		isPlaying = false;
		PlayerPrefs.SetInt("LastScore", getScore());

		CancelInvoke ("UpdateScore");
		CancelInvoke ("CreateObstacle");
		score = 0;
		speed = sourceSpeed;
		playerObject.isDying = false;
		playerObject.rigidbody2D.velocity = Vector3.zero;
		playerObject.rigidbody2D.angularVelocity = 0f;

		playerObject.transform.position = new Vector3(Camera.main.transform.position.x-7f,5.4f,playerObject.transform.position.z);
		playerObject.transform.rotation = new Quaternion (0f,0f,0f,0f);

		foreach(GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle")) {
			Destroy (obstacle);
		}

	}

	void CreateObstacle () {
		float i = Random.value * obstacles.Length;
		GameObject item = obstacles[(int)Mathf.Floor(i)];
		Instantiate(item);
	}

	public static void stopBGM() {

	}

	private IEnumerator getConfig() {
		WWW www = new WWW("http://cluckbutton.localhost/api/config");
		yield return www;
		
		if (!string.IsNullOrEmpty (www.text)) {
			Debug.Log(www.text);
			Debug.Log(Json.Deserialize(www.text));
		}
	}

	private void FakeFly() {
		//playerObject.GetComponent ("Script").Jump ();
		var jumpForce = new Vector2 (0,20);
		playerObject.rigidbody2D.velocity = Vector3.zero;
		playerObject.rigidbody2D.AddForce(jumpForce);
		//Player.Jump ();
	}

}