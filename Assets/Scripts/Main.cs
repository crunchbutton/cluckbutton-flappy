using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;

public class Main : MonoBehaviour {

	public GUIStyle scoreStyle;
	public GUIStyle logoStyle;
	public GameObject[] obstacles;

	public Player playerObject;

	public GUIStyle startButton;

	
	private int score = 0;
	private int scoreToSkip = 2;
	private double sourceSpeed = .1;
	private int incrimentAt = 10;
	private bool hasPlayed = false;

	public static GameObject BGM;
	public static float obstacleSpeed = 1.5f;
	public static float obstacleSpeedOffset = 1f;
	public static double speed = .1;

	private string GUID;

	private string labelTitle;
	private string labelPlay;




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

		GUID = PlayerPrefs.GetString ("GUID");

		if (GUID == "") {
			Debug.Log("new guid");
			System.Guid newGUID = System.Guid.NewGuid();
			GUID = newGUID.ToString();
			PlayerPrefs.SetString("GUID", GUID);
		}

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
		if (hasPlayed) {
			GUILayout.Label("Score: " + getScore().ToString(), scoreStyle);
		}

		if (!isPlaying) {
			if (hasPlayed) {

				labelTitle = "Game Over";
				labelPlay = "Try Again";

				if (GUI.Button(new Rect(10,20,100,60),"Init")) {
					//init ();
				}
				
				if (GUI.Button(new Rect(10,100,100,60),"Login")) {
					//login ();
				}
				
				if (GUI.Button(new Rect(10,180,100,60),"Logout")) {
					//logout ();
				}

			} else {
				labelTitle = "Cluckbutton";
				labelPlay = "Start Clucking";


			}

			GUI.Label (new Rect (Screen.width/2-50, Screen.height/2-100, 100, 50), labelTitle, logoStyle);

			if (GUI.Button (new Rect (Screen.width/2-125, Screen.height/2+20, 250, 50), labelPlay, startButton)) {
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

		StartCoroutine (reportScore ());

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

	IEnumerator reportScore() {
		WWWForm form = new WWWForm();
		form.AddField("fb", "123", System.Text.Encoding.UTF8);
		form.AddField("score", getScore().ToString(), System.Text.Encoding.UTF8);
		WWW www = new WWW("http://cluckbutton.localhost/api/score", form);
		
		yield return www;
		
		if (!string.IsNullOrEmpty(www.text)) {
			Debug.Log(www.text);
		}
	}
	/*
	public static string GetUniqueID(){
		string key = "ID";
		
		var random = new System.Random();              
		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		
		string uniqueID = Application.systemLanguage                 //Language
			+"-"+GetPlatform()                           //Device   
				+"-"+String.Format("{0:X}", Convert.ToInt32(timestamp))          //Time
				+"-"+String.Format("{0:X}", Convert.ToInt32(Time.time*1000000))     //Time in game
				+"-"+String.Format("{0:X}", random.Next(1000000000));          //random number
		
		Debug.Log("Generated Unique ID: "+uniqueID);
		
		if(PlayerPrefs.HasKey(key)){
			uniqueID = PlayerPrefs.GetString(key);      
		} else {       
			PlayerPrefs.SetString(key, uniqueID);
			PlayerPrefs.Save();  
		}
		
		return uniqueID;
	}
	*/

}