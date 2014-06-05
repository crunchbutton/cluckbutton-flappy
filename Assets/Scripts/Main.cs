using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;

public class Main : MonoBehaviour {

	public GUIStyle hiScoreStyle;
	public GUIStyle scoreStyle;
	public GUIStyle logoStyle;
	public GameObject[] obstacles;

	public Player playerObject;

	public GUIStyle startButton;
	public GUIStyle promoButton;
	
	private int score = 0;
	private int scoreToSkip = 2;
	private double sourceSpeed = .08;
	private float obstacleSourceSpeed = 2f;
	private int incrimentAt = 10;
	private bool hasPlayed = false;
	private float uiScale;
	private int bestScore = 0;
	private bool promoPage = false;

	public static GameObject BGM;
	public static float obstacleSpeed;
	public static float obstacleSpeedOffset = 1f;
	public static double speed;

	private string GUID;

	private string labelTitle;
	private string labelPlay;

	public static bool isPlaying = false;


	void Update() {
		if (!isPlaying && !hasPlayed) {
			/*
			if (Input.GetMouseButtonDown(0) || Input.GetKeyUp("space")) {
				play();
			} else if (Input.touchCount > 0) {
				for (int i = 0; i < Input.touchCount; i++) {
					if (Input.touches[i].phase == TouchPhase.Began) {
						play();
						break;
					}
				}
			}
*/
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
		bestScore = PlayerPrefs.GetInt ("BestScore");

		if (GUID == "") {
			Debug.Log("new guid");
			System.Guid newGUID = System.Guid.NewGuid();
			GUID = newGUID.ToString();
			PlayerPrefs.SetString("GUID", GUID);
		}

		if (Screen.dpi >= 320) {
			speed = sourceSpeed = sourceSpeed * 2;
			//obstacleSpeed = obstacleSourceSpeed = 1.8f;
			obstacleSpeed = obstacleSourceSpeed;
		} else {
			speed = sourceSpeed;
			obstacleSpeed = obstacleSourceSpeed;
		}

		if (Screen.dpi < 320) {
			//uiScale = 320 / Screen.dpi;
			uiScale = .8f;
		} else {
			uiScale = 1f;
		}

		logoStyle.fontSize = (int)(logoStyle.fontSize * uiScale);
		scoreStyle.fontSize = (int)(scoreStyle.fontSize * uiScale);
		hiScoreStyle.fontSize = (int)(hiScoreStyle.fontSize * uiScale);
		hiScoreStyle.fixedWidth = Screen.width;

		startButton.fontSize = (int)(startButton.fontSize * uiScale);
		startButton.fixedWidth = (int)(startButton.fixedWidth * uiScale);

		promoButton.fontSize = (int)(promoButton.fontSize * uiScale);
		promoButton.fixedWidth = (int)(promoButton.fixedWidth * uiScale);


		BGM = GameObject.Find("BGM");
		StartCoroutine (getConfig ());
		FB.Init(FBInitComplete);

		reset (true);
	}
	
	void OnDestroy () {
		//reset ();
	}
	
	void OnGUI () {
		if (promoPage) {
			GUI.Label (new Rect (Screen.width/2-50, Screen.height/2-(150 * uiScale), 100, 100 * uiScale), "Free Stuff", logoStyle);

		} else {
			if (hasPlayed) {
				GUILayout.Label("Score: " + getScore().ToString(), scoreStyle);
				hiScoreStyle.contentOffset = new Vector2(0f, -(hiScoreStyle.fontSize * uiScale + (hiScoreStyle.padding.top * 2)));
			}

			if (bestScore > 0) {
				GUILayout.Label("Best: " + bestScore.ToString(), hiScoreStyle);
			}

			if (!isPlaying) {
				if (hasPlayed) {

					labelTitle = "Game Over";
					labelPlay = "Try Again";
					
					if (GUI.Button(new Rect(10,100,200,60),"Login")) {
						FB.Login("", FBLoginComplete);
					}
					
					if (GUI.Button(new Rect(10,180,200,60),"Logout")) {
						FB.Logout();
					}

				} else {
					labelTitle = "Cluck Button";
					labelPlay = "Start Clucking";
				}

				GUI.Label (new Rect (Screen.width/2-50, Screen.height/2-(150 * uiScale), 100, 100 * uiScale), labelTitle, logoStyle);

				if (GUI.Button (new Rect ((Screen.width/2)-(startButton.fixedWidth/2), Screen.height/2+(30*uiScale), startButton.fixedWidth, 74 * uiScale), labelPlay, startButton)) {
					play();
				}

				if (hasPlayed) {
					if (GUI.Button (new Rect ((Screen.width/2)-(promoButton.fixedWidth/2), Screen.height/2+(120*uiScale), promoButton.fixedWidth, 74 * uiScale), "Get Free Delivery Food", promoButton)) {
						promoPage = true;
					}
				}
			}
		}
	}

	void UpdateScore() {
		score++;
		// update the scroll speed
		speed = sourceSpeed + (getScore() / incrimentAt) * .01;

		// update the obstacle speed
		//obstacleSpeed = obstacleSourceSpeed + (getScore() / incrimentAt) * .01;

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

		InvokeRepeating("FakeFly", 2f, 4.07f);

		BGM.audio.Stop ();
		isPlaying = false;
		if (getScore () > bestScore) {
			PlayerPrefs.SetInt("BestScore", getScore());
		}
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

	void FBInitComplete() {

	}

	void FBLoginComplete(FBResult result) {
		if (result.Error != null) {
			Debug.Log(result.Error);
		} else if (!FB.IsLoggedIn) {
			Debug.Log("canceled");
		} else {
			Debug.Log("success");
		}
	}


}